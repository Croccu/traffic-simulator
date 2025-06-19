using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarDetectionLogic), typeof(CarEnvironmentTriggers))]
public class CarController_v3 : MonoBehaviour
{
    [Header("Speed Settings")]
    public float maxSpeed = 5f;
    public float accelerationRate = 3f;
    public float rotationSpeed = 280f;
    public float waypointReachThreshold = 0.05f;
    public float bezierStep = 0.02f;

    [Header("Lane Switching")]
    public float parallelDetectRange = 2f;
    public float parallelAngleThreshold = 20f;
    public float switchCooldown = 1.5f;
    public bool debugDrawSwitch = false;
    public float switchDuration = 0.5f;

    [Header("Detection")]
    public float detectionLength = 1.4f;
    public float detectionWidth = 0.6f;
    public float stopDistance = 0.7f;
    public float laneMergeCheckRadius = 0.6f;
    public float rearDetectLength = 1f;
    public float rearDetectWidth = 0.6f;

    public WaypointNode currentNode;
    public BezierWaypointSegment currentSegment;

    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentSpeedLimit;
    [HideInInspector] public bool isSwitchingLanes;
    [HideInInspector] public bool inMergeBlock;
    [HideInInspector] public bool isMoving;

    private float targetSpeed;
    private float switchTimer = 0f;
    private float lastSwitchTime = -999f;
    private float timeStuckAtSameNode = 0f;
    private int lastStuckPathIndex = -1;
    private float fullStopTimer = 0f;

    private List<Vector3> bezierPath;
    private int pathIndex = 0;
    private List<Vector3> switchNewPath;
    private Collider2D selfCollider;

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

        bool blocked = GetComponent<CarEnvironmentTriggers>().IsEnvironmentBlocking();
        if (blocked == false)
            GetComponent<CarEnvironmentTriggers>().ClearGiveWayFlagIfPossible();

        targetSpeed = blocked ? 0f : currentSpeedLimit;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationRate);

        if (inMergeBlock && GetComponent<CarDetectionLogic>().EvaluateMergeUnblock(this))
            return;

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

        GetComponent<CarDetectionLogic>().AdjustSpeedBasedOnCarAhead(this);

        if (GetComponent<CarDetectionLogic>().IsRearCarTooClose(this))
        {
            currentSpeed = 0f;
            return;
        }

        if (pathIndex == lastStuckPathIndex)
        {
            timeStuckAtSameNode += Time.deltaTime;
            if (timeStuckAtSameNode > 5f)
            {
                GetComponent<CarDetectionLogic>().TryLaneSwitchOrExitDeadlock(this);
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
                if (currentNode != null && currentNode.isExit && !GetComponent<CarDetectionLogic>().IsBlockedAtEntry(bezierPath, this))
                {
                    GameManager.instance?.CarDespawned();
                    Destroy(gameObject);
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
        }
        else
        {
            fullStopTimer = 0f;
        }
    }

    void AdvanceToNextSegment()
    {
        if (currentNode == null || currentNode.outgoingCurves.Count == 0)
        {
            isMoving = false;
            return;
        }

        BezierWaypointSegment nextCurve = GetComponent<CarDetectionLogic>().PickUnoccupiedOrRandomCurve(currentNode.outgoingCurves, this);
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

        BezierWaypointSegment segment = GetComponent<CarDetectionLogic>().PickUnoccupiedOrRandomCurve(currentNode.outgoingCurves, this);
        GenerateBezierPathFrom(segment);
        currentNode = segment.endNode;
        inMergeBlock = true;
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
}
