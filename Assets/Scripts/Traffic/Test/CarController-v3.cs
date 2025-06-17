using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController_v3 : MonoBehaviour
{
    [Header("Settings")]
    public float maxSpeed = 5f;
    public float rotationSpeed = 280f;
    public float waypointReachThreshold = 0.05f;
    public float bezierStep = 0.02f;

    [Header("Detection")]
    public LayerMask carLayer;
    public float detectionRadius = 2f;
    public float stopDistance = 1f;

    private float currentSpeed;
    private float currentSpeedLimit;
    private bool isMoving = false;

    public WaypointNode currentNode;

    private List<Vector3> bezierPath;
    private int pathIndex = 0;

    private Collider2D selfCollider;

    private bool waitingAtGiveWay = false;
    private GiveWayZone currentGiveWayZone = null;

    private bool atTrafficLight = false;
    private TrafficLightLogic currentTrafficLight = null;

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

        // Traffic light logic
        if (atTrafficLight && currentTrafficLight?.CurrentState == TrafficLightLogic.LightState.Red)
        {
            currentSpeed = 0f;
            return;
        }

        // Give way logic
        if (waitingAtGiveWay && currentGiveWayZone != null)
        {
            if (currentGiveWayZone.IsIntersectionClear(selfCollider))
                waitingAtGiveWay = false;
            else
            {
                currentSpeed = 0f;
                return;
            }
        }

        AdjustSpeedBasedOnCarAhead();

        Vector3 target = bezierPath[pathIndex];
        float distance = Vector3.Distance(transform.position, target);

        if (distance < waypointReachThreshold)
        {
            pathIndex++;

            if (pathIndex >= bezierPath.Count)
            {
                // ✅ Despawn here if current node is an exit
                if (currentNode != null && currentNode.isExit)
                {
                    GameManager.instance.CarDespawned();
                    Destroy(gameObject);
                    return;
                }

                AdvanceToNextSegment();
                return;
            }

            target = bezierPath[pathIndex];
        }

        transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);

        Vector3 direction = target - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void AdvanceToNextSegment()
    {
        if (currentNode == null || currentNode.nextNodes.Count == 0)
        {
            isMoving = false;
            return;
        }

        currentNode = currentNode.nextNodes[Random.Range(0, currentNode.nextNodes.Count)];
        GenerateNextBezierPath();
    }

    void GenerateNextBezierPath()
    {
        BezierWaypointSegment segment = currentNode.GetComponent<BezierWaypointSegment>();
        if (segment == null || segment.endNode == null)
        {
            Debug.LogWarning("No valid Bezier segment found on node: " + currentNode.name);
            isMoving = false;
            return;
        }

        Vector3 p0 = currentNode.transform.position;
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
        currentNode = segment.endNode;
    }

    void AdjustSpeedBasedOnCarAhead()
    {
        Vector2 detectionPoint = (Vector2)transform.position + (Vector2)transform.up * (detectionRadius / 2);
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint, detectionRadius / 2, carLayer);

        bool carDetected = false;
        float closestDistance = float.MaxValue;

        foreach (Collider2D col in hits)
        {
            if (col != selfCollider)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < closestDistance)
                    closestDistance = dist;

                carDetected = true;
            }
        }

        if (carDetected)
        {
            if (closestDistance < stopDistance)
                currentSpeed = 0f;
            else
            {
                float t = (closestDistance - stopDistance) / (detectionRadius - stopDistance);
                currentSpeed = Mathf.Lerp(0f, currentSpeedLimit, t);
            }
        }
        else
        {
            currentSpeed = currentSpeedLimit;
        }
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

    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + transform.up * (detectionRadius / 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, detectionRadius / 2);
    }
}
