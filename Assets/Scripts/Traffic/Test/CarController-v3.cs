using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

public class CarController_v3 : MonoBehaviour
{
    [Header("Settings")]
    public float maxSpeed = 3.5f;
    public float rotationSpeed = 200f;
    public float waypointReachThreshold = 0.05f;
    public float bezierStep = 0.02f;

    [Header("Speed Tuning")]
    public float accelerationRate = 3f;
    private float targetSpeed;

    [Header("Detection")]
    public LayerMask carLayer;
    public float detectionLength = 1.4f;
    public float detectionWidth = 0.6f;
    public float stopDistance = 0.7f;

    [Header("Lane Switching")]
    public float parallelDetectRange = 2f;
    public float parallelAngleThreshold = 20f;
    public float switchCooldown = 1.5f;
    public bool debugDrawSwitch = false;

    [Header("Lane Switch Tuning")]
    public float switchDuration = 0.5f;
    [Range(0.1f, 2f)]
    public float laneMergeCheckRadius = 0.6f;

    [Header("Rear Detection")]
    public float rearDetectLength = 1f;
    public float rearDetectWidth = 0.6f;

    public WaypointNode currentNode;
    public BezierWaypointSegment currentSegment;

    private float currentSpeed;
    private float currentSpeedLimit;
    private bool isMoving = false;

    private List<Vector3> bezierPath;
    private int pathIndex = 0;

    private Collider2D selfCollider;

    private bool waitingAtGiveWay = false;
    private GiveWayZone currentGiveWayZone = null;

    private bool atTrafficLight = false;
    private TrafficLightLogic currentTrafficLight = null;

    private float lastSwitchTime = -999f;

    private bool isSwitchingLanes = false;
    private List<Vector3> switchNewPath;
    private float switchTimer = 0f;

    private bool inMergeBlock = false;
    private float blockedTimer = 0f;

    private float timeStuckAtSameNode = 0f;
    private int lastStuckPathIndex = -1;

    private float fullStopTimer = 0f;

    //For debugging
    private bool hasLoggedStuck = false;
    private bool hasLoggedFrontStop = false;
    private bool hasLoggedRearThreat = false;
    private bool hasLoggedMergeBlock = false;

    void Reset()
    {
    maxSpeed = 3.5f;
    rotationSpeed = 200f;
    waypointReachThreshold = 0.05f;
    bezierStep = 0.02f;
    }

    public void SetStartNode(WaypointNode start)
    {
      currentNode = start;
      currentSpeedLimit = maxSpeed;
      currentSpeed = currentSpeedLimit;
      selfCollider = GetComponent<Collider2D>();

      GenerateNextBezierPath();
      isMoving = true;
    }

    void Update()
    {
        if (!isMoving || bezierPath == null || pathIndex >= bezierPath.Count) return;

        bool blocked =
            (atTrafficLight && currentTrafficLight?.CurrentState == TrafficLightLogic.LightState.Red) ||
            (waitingAtGiveWay && currentGiveWayZone != null && !currentGiveWayZone.IsIntersectionClear(selfCollider));

        if (waitingAtGiveWay && currentGiveWayZone != null && currentGiveWayZone.IsIntersectionClear(selfCollider))
            waitingAtGiveWay = false;

        targetSpeed = blocked ? 0f : currentSpeedLimit;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationRate);

        if (inMergeBlock)
        {
            if (pathIndex > 3 || !IsBlockedAtEntry(bezierPath))
            {
                inMergeBlock = false;
                blockedTimer = 0f;
            }
            else
            {
                Collider2D hit = Physics2D.OverlapCapsule(
                    (Vector2)transform.position + (Vector2)transform.up * (detectionLength * 0.5f),
                    new Vector2(detectionWidth, detectionLength),
                    CapsuleDirection2D.Vertical,
                    transform.eulerAngles.z,
                    carLayer
                );

                bool physicallyClear = (hit == null || hit == selfCollider);
                if (!physicallyClear)
                {
                    CarController_v3 carAhead = hit.GetComponent<CarController_v3>();
                    if (carAhead == this) return;

                    // Directional check
                    if (carAhead != null)
                    {
                        Vector2 myForward = transform.up;
                        Vector2 theirForward = carAhead.transform.up;

                        float dirDot = Vector2.Dot(myForward.normalized, theirForward.normalized);
                        if (dirDot < 0.5f)
                        {
                            // Oncoming car — ignore
                            return;
                        }
                    }

                    float dist = Vector2.Distance(transform.position, hit.transform.position);
                    if (carAhead == null || carAhead.pathIndex > this.pathIndex + 5 || dist > 1.5f)
                    {
                        inMergeBlock = false;
                        blockedTimer = 0f;
                    }
                }

                if (inMergeBlock)
                {
                    blockedTimer += Time.deltaTime;
                    if (blockedTimer > 3f && !hasLoggedMergeBlock)
                        Debug.LogWarning($"{name} STUCK BLOCK (>3s) at merge entry — check logic or path");
                    hasLoggedMergeBlock = true;
                    currentSpeed = 0f;
                    return;
                }
                else {
                    hasLoggedMergeBlock = false;
                }
            }
        }

        if (isSwitchingLanes)
        {
            switchTimer += Time.deltaTime;
            float t = Mathf.Clamp01(switchTimer / switchDuration);
            Vector3 current = bezierPath[Mathf.Min(pathIndex, bezierPath.Count - 1)];
            Vector3 target = switchNewPath[Mathf.Min(pathIndex, switchNewPath.Count - 1)];
            Vector3 blended = Vector3.Lerp(current, target, t);

            transform.position = Vector3.MoveTowards(transform.position, blended, currentSpeed * Time.deltaTime);

            Vector3 dir = target - transform.position;
            if (dir.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRot = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            if (t >= 1f)
            {
                isSwitchingLanes = false;
                bezierPath = switchNewPath;
            }

            return;
        }

        AdjustSpeedBasedOnCarAhead();
        if (IsRearCarTooClose())
        {
            currentSpeed = 0f;
            return;
        }

        if (pathIndex == lastStuckPathIndex)
        {
            timeStuckAtSameNode += Time.deltaTime;
            if (timeStuckAtSameNode > 5f)
            {
                TryLaneSwitchOrExitDeadlock();
                timeStuckAtSameNode = 0f;
            }
        }
        else
        {
            timeStuckAtSameNode = 0f;
            lastStuckPathIndex = pathIndex;
        }

        Vector3 point = bezierPath[pathIndex];
        float distance = Vector3.Distance(transform.position, point);

        if (distance < waypointReachThreshold)
        {
            pathIndex++;

            if (pathIndex >= bezierPath.Count)
            {
                if (currentNode != null && currentNode.isExit)
                {
                    if (!IsBlockedAtEntry(bezierPath))
                    {
                        GameManager.instance?.CarDespawned();
                        Destroy(gameObject);
                    }
                    return;
                }

                AdvanceToNextSegment();
                return;
            }

            point = bezierPath[pathIndex];
        }

        transform.position = Vector3.MoveTowards(transform.position, point, currentSpeed * Time.deltaTime);

        Vector3 direction = point - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (currentSpeed <= 0.01f)
        {
            fullStopTimer += Time.deltaTime;
            if (fullStopTimer > 6f)
            {
                Debug.LogWarning($"{name} DESPAWN after 6s stall");
                GameManager.instance?.CarDespawned();
                Destroy(gameObject);
            }

            if (!hasLoggedStuck)
            {
                Debug.Log($"{name} is STUCK or STALLED — Path index: {pathIndex}, Node: {currentNode?.name}");
                hasLoggedStuck = true;
            }
        }
        else
        {
            fullStopTimer = 0f;
            hasLoggedStuck = false;
        }
    }


    void TryLaneSwitchOrExitDeadlock()
    {
        BezierWaypointSegment alt = FindParallelOpenSegment();
        if (alt != null && IsSafeToSwitchTo(alt))
        {
            lastSwitchTime = Time.time;
            StartCoroutine(SmoothSwitchTo(alt));
        }
    }


    void AdvanceToNextSegment()
    {
        if (currentNode == null || currentNode.outgoingCurves.Count == 0)
        {
            isMoving = false;
            return;
        }

        BezierWaypointSegment nextCurve = PickUnoccupiedOrRandomCurve(currentNode.outgoingCurves);
        currentNode = nextCurve.endNode;
        GenerateBezierPathFrom(nextCurve);
        inMergeBlock = true;
    }

    void GenerateNextBezierPath()
    {
        if (currentNode == null || currentNode.outgoingCurves.Count == 0)
        {
            isMoving = false;
            return;
        }

        BezierWaypointSegment segment = PickUnoccupiedOrRandomCurve(currentNode.outgoingCurves);
        GenerateBezierPathFrom(segment);
        currentNode = segment.endNode;
        inMergeBlock = true;
    }

    BezierWaypointSegment PickUnoccupiedOrRandomCurve(List<BezierWaypointSegment> segments)
    {
        foreach (var seg in segments)
        {
            Vector3 mid = Vector3.Lerp(seg.transform.position, seg.endNode.transform.position, 0.5f);
            Collider2D[] hits = Physics2D.OverlapCircleAll(mid, laneMergeCheckRadius, carLayer);
            bool clear = true;
            foreach (var col in hits)
            {
                if (col != null && col != selfCollider)
                {
                    clear = false;
                    break;
                }
            }

            if (clear) return seg;
        }

        return segments[Random.Range(0, segments.Count)];
    }

    void GenerateBezierPathFrom(BezierWaypointSegment segment)
    {
        currentSegment = segment;

        Vector3 p0 = segment.transform.position;
        Vector3 p1 = segment.controlPoint;
        Vector3 p2 = segment.endNode.transform.position;

        bezierPath = new List<Vector3>();
        for (float t = 0; t <= 1f; t += bezierStep)
        {
            Vector3 point = Mathf.Pow(1 - t, 2) * p0 +
                            2 * (1 - t) * t * p1 +
                            Mathf.Pow(t, 2) * p2;
            bezierPath.Add(point);
        }

        pathIndex = 0;
    }

    void AdjustSpeedBasedOnCarAhead()
    {
        Vector2 center = (Vector2)transform.position + (Vector2)transform.up * (detectionLength * 0.5f);
        Vector2 size = new Vector2(detectionWidth, detectionLength);
        float angle = transform.eulerAngles.z;

        Collider2D hit = Physics2D.OverlapCapsule(center, size, CapsuleDirection2D.Vertical, angle, carLayer);

        if (hit != null && hit != selfCollider)
        {
            CarController_v3 carAhead = hit.GetComponent<CarController_v3>();

            // Ignore if not same segment
            if (carAhead != null && carAhead.currentSegment != this.currentSegment)
                return;

            // Directional check: make sure it's forward-moving
            if (carAhead != null)
            {
                Vector2 myForward = transform.up;
                Vector2 theirForward = carAhead.transform.up;
                float dirDot = Vector2.Dot(myForward.normalized, theirForward.normalized);

                if (dirDot < 0.75f)
                    return;
            }

            float dist = Vector2.Distance(transform.position, hit.transform.position);

            if (Time.time - lastSwitchTime > switchCooldown)
            {
                BezierWaypointSegment alt = FindParallelOpenSegment();
                if (alt != null && IsSafeToSwitchTo(alt))
                {
                    if (debugDrawSwitch)
                        Debug.DrawLine(transform.position, alt.transform.position, Color.green, 1f);

                    lastSwitchTime = Time.time;
                    StartCoroutine(SmoothSwitchTo(alt));
                    return;
                }
            }

            if (dist < stopDistance)
            {
                if (!hasLoggedFrontStop)
                {
                    Debug.Log($"{name} FULL STOP — {hit.name} ahead at {dist:F2} units");
                    hasLoggedFrontStop = true;
                }
                currentSpeed = 0f;
            }
            else
            {
                hasLoggedFrontStop = false;
                float t = (dist - stopDistance) / (detectionLength - stopDistance);
                currentSpeed = Mathf.Lerp(0f, currentSpeedLimit, t);
            }
        }
        else
        {
            hasLoggedFrontStop = false;
            currentSpeed = currentSpeedLimit;
        }
    }



    bool IsRearCarTooClose()
    {
        Vector2 backCenter = (Vector2)transform.position - (Vector2)transform.up * (rearDetectLength * 0.5f);
        Vector2 rearSize = new Vector2(rearDetectWidth, rearDetectLength);
        float angle = transform.eulerAngles.z;

        Collider2D rearHit = Physics2D.OverlapCapsule(backCenter, rearSize, CapsuleDirection2D.Vertical, angle, carLayer);
        if (rearHit != null && rearHit != selfCollider)
        {
            Rigidbody2D rb = rearHit.attachedRigidbody;
            if (rb != null)
            {
                Vector2 toMe = (Vector2)transform.position - rb.position;
                Vector2 rearVel = rb.linearVelocity;

                if (rearVel.magnitude > 0.01f)
                {
                    float approachDot = Vector2.Dot(rearVel.normalized, toMe.normalized);
                    float distance = Vector2.Distance(rb.position, transform.position);

                    if (approachDot > 0.7f && distance < rearDetectLength)
                    {
                        if (!hasLoggedRearThreat)
                        {
                            Debug.Log($"{name} STOPPED due to REAR THREAT: {rearHit.name}, dist: {distance:F2}, dot: {approachDot:F2}");
                            hasLoggedRearThreat = true;
                        }
                        return true;
                    }
                    else
                    {
                        hasLoggedRearThreat = false;
                    }
                }
            }
        }

        return false;
    }



    IEnumerator SmoothSwitchTo(BezierWaypointSegment segment)
    {
        isSwitchingLanes = true;
        switchTimer = 0f;
        currentSegment = segment;

        Vector3 p0 = segment.transform.position;
        Vector3 p1 = segment.controlPoint;
        Vector3 p2 = segment.endNode.transform.position;

        switchNewPath = new List<Vector3>();
        for (float t = 0; t <= 1f; t += bezierStep)
        {
            Vector3 point = Mathf.Pow(1 - t, 2) * p0 +
                            2 * (1 - t) * t * p1 +
                            Mathf.Pow(t, 2) * p2;
            switchNewPath.Add(point);
        }

        currentNode = segment.endNode;
        inMergeBlock = true;
        yield return null;
    }

    bool IsSafeToSwitchTo(BezierWaypointSegment segment)
    {
        Vector3 p0 = segment.transform.position;
        Vector3 p1 = segment.controlPoint;
        Vector3 p2 = segment.endNode.transform.position;

        for (float t = 0f; t <= 0.4f; t += 0.1f)
        {
            Vector3 point = Mathf.Pow(1 - t, 2) * p0 +
                            2 * (1 - t) * t * p1 +
                            Mathf.Pow(t, 2) * p2;

            Collider2D[] hits = Physics2D.OverlapCircleAll(point, laneMergeCheckRadius, carLayer);
            foreach (var col in hits)
            {
                if (col != null && col != selfCollider)
                    return false;
            }
        }

        return true;
    }

    bool IsBlockedAtEntry(List<Vector3> path)
    {
        int checkCount = Mathf.Min(5, path.Count);
        for (int i = 0; i < checkCount; i++)
        {
            int idx = Mathf.FloorToInt((i / (float)checkCount) * path.Count);
            Vector3 pos = path[idx];
            Collider2D[] hits = Physics2D.OverlapCircleAll(pos, laneMergeCheckRadius, carLayer);

            foreach (var col in hits)
            {
                if (col == null || col == selfCollider)
                    continue;

                CarController_v3 other = col.GetComponent<CarController_v3>();
                if (other == null)
                    continue;

                // Ignore unrelated segments
                if (other.currentSegment != this.currentSegment)
                    continue;

                // Ignore opposite-direction cars
                Vector2 myForward = transform.up;
                Vector2 theirForward = other.transform.up;
                float dot = Vector2.Dot(myForward.normalized, theirForward.normalized);
                if (dot < 0.75f)
                    continue;

                return true; // Blocked by valid same-segment, same-direction car
            }
        }
        return false;
    }



    BezierWaypointSegment FindParallelOpenSegment()
    {
        BezierWaypointSegment[] allSegments = Object.FindObjectsByType<BezierWaypointSegment>(FindObjectsSortMode.None);
        Vector2 forward = transform.up;

        foreach (var seg in allSegments)
        {
            if (seg == null || seg.endNode == null) continue;

            Vector3 toSeg = seg.transform.position - transform.position;
            if (toSeg.magnitude > parallelDetectRange) continue;
            if (Vector3.Dot(toSeg.normalized, forward) < 0.5f) continue;

            Vector2 segDir = (seg.endNode.transform.position - seg.transform.position).normalized;
            float angleDiff = Vector2.Angle(forward, segDir);
            if (angleDiff > parallelAngleThreshold) continue;

            Vector3 testCenter = seg.transform.position + (Vector3)(segDir * detectionLength * 0.5f);
            Vector2 size = new Vector2(detectionWidth, detectionLength);
            Collider2D blocker = Physics2D.OverlapCapsule(testCenter, size, CapsuleDirection2D.Vertical, seg.transform.eulerAngles.z, carLayer);

            if (blocker == null)
                return seg;
        }

        return null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var zone = other.GetComponent<GiveWayZone>();
        if (zone != null)
        {
            currentGiveWayZone = zone;
            waitingAtGiveWay = true;
        }

        var speedZone = other.GetComponent<SpeedZone>();
        if (speedZone != null)
        {
            currentSpeedLimit = speedZone.speedLimit;
        }

        if (other.CompareTag("TrafficLightTrigger"))
        {
            currentTrafficLight = other.GetComponentInParent<TrafficLightLogic>();
            if (currentTrafficLight != null)
                atTrafficLight = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<GiveWayZone>() == currentGiveWayZone)
        {
            currentGiveWayZone = null;
            waitingAtGiveWay = false;
        }

        if (other.CompareTag("TrafficLightTrigger"))
        {
            if (other.GetComponentInParent<TrafficLightLogic>() == currentTrafficLight)
            {
                atTrafficLight = false;
                currentTrafficLight = null;
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector2 center = (Vector2)transform.position + (Vector2)transform.up * (detectionLength * 0.5f);
        Vector2 size = new Vector2(detectionWidth, detectionLength);
        float angle = transform.eulerAngles.z;

        Gizmos.color = new Color(1f, 0.2f, 0f, 0.3f);
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;

        Vector2 backCenter = (Vector2)transform.position - (Vector2)transform.up * (rearDetectLength * 0.5f);
        Vector2 rearSize = new Vector2(rearDetectWidth, rearDetectLength);
        Matrix4x4 rearMatrix = Matrix4x4.TRS(backCenter, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.color = new Color(0f, 0.3f, 1f, 0.2f);
        Gizmos.matrix = rearMatrix;
        Gizmos.DrawCube(Vector3.zero, rearSize);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
// Note: This code is a continuation of the original CarController script, which handles car movement, lane switching, and interaction with waypoints and traffic systems in a Unity game environment.
// It includes features like bezier path generation, speed adjustment based on nearby cars, and detection of give way zones and traffic lights.
// The script is designed to be used in a traffic simulation context, where cars navigate through a network of waypoints while adhering to traffic rules and responding to other vehicles.
// The code also includes debugging features to visualize detection areas and lane switching logic, making it easier to test and refine the car behavior in the game environment.
