using System.Collections.Generic;
using UnityEngine;

public class BezierCarController : MonoBehaviour
{
    private List<Vector3> path;
    private int currentIndex = 0;

    [Header("Car Settings")]
    public float maxSpeed = 5f;
    public float rotationSpeed = 5f;
    public float detectionRadius = 2f;
    public float stopDistance = 1f;

    private float currentSpeed;
    private bool isMoving = false;

    [Header("Detection")]
    public LayerMask carLayer;

    private Collider2D selfCollider;

    private bool waitingAtGiveWay = false;
    private GiveWayZone currentGiveWayZone = null;

    public BezierRouteSpline currentSpline;

    void Start()
    {
        currentSpeed = maxSpeed;
        selfCollider = GetComponent<Collider2D>();
    }

    public void InitializePath(List<Vector3> pathPoints)
    {
        if (pathPoints == null || pathPoints.Count < 2)
            return;

        path = pathPoints;
        currentIndex = 0;
        transform.position = path[0];
        isMoving = true;
        currentSpeed = maxSpeed;
    }

    public void SetCurrentSpline(BezierRouteSpline spline)
    {
        currentSpline = spline;
    }

    void Update()
    {
        if (!isMoving || path == null || currentIndex >= path.Count)
            return;

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

        Vector3 target = path[currentIndex];
        Vector3 direction = target - transform.position;

        if (direction.magnitude < 0.05f)
            currentIndex++;

        if (currentIndex >= path.Count)
        {
            TrySwitchToNextSpline();
            return;
        }

        transform.position += direction.normalized * currentSpeed * Time.deltaTime;

        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void TrySwitchToNextSpline()
    {
        if (currentSpline != null && currentSpline.nextSplines.Count > 0)
        {
            List<BezierRouteSpline> validOptions = new List<BezierRouteSpline>();
            foreach (var next in currentSpline.nextSplines)
            {
                if (next != null)
                {
                    next.GenerateWaypoints();
                    if (next.waypoints != null && next.waypoints.Count >= 2)
                        validOptions.Add(next);
                }
            }

            if (validOptions.Count > 0)
            {
                BezierRouteSpline chosen = validOptions[Random.Range(0, validOptions.Count)];
                currentSpline = chosen;
                InitializePath(new List<Vector3>(chosen.waypoints));
                return;
            }
        }

        isMoving = false;
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
                currentSpeed = Mathf.Lerp(0f, maxSpeed, t);
            }
        }
        else
            currentSpeed = maxSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GiveWayZone zone = other.GetComponent<GiveWayZone>();
        if (zone != null)
        {
            currentGiveWayZone = zone;
            waitingAtGiveWay = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<GiveWayZone>() == currentGiveWayZone)
        {
            currentGiveWayZone = null;
            waitingAtGiveWay = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + transform.up * (detectionRadius / 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, detectionRadius / 2);
    }
}
