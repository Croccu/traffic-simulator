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

    // Give Way
    private bool waitingAtGiveWay = false;
    private GiveWayZone currentGiveWayZone = null;

    void Start()
    {
        currentSpeed = maxSpeed;
        selfCollider = GetComponent<Collider2D>();
    }

    public void InitializePath(List<Vector3> pathPoints)
    {
        if (pathPoints == null || pathPoints.Count < 2)
        {
            Debug.LogError("Path is invalid or too short.");
            return;
        }

        path = pathPoints;
        currentIndex = 0;
        transform.position = path[0];
        isMoving = true;
        currentSpeed = maxSpeed;
    }

    void Update()
    {
        if (!isMoving || path == null || currentIndex >= path.Count)
            return;

        // Yield logic
        if (waitingAtGiveWay && currentGiveWayZone != null)
        {
            if (currentGiveWayZone.IsIntersectionClear(selfCollider))
            {
                waitingAtGiveWay = false;
            }
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
        {
            currentIndex++;
            if (currentIndex >= path.Count)
            {
                isMoving = false;
                return;
            }
            target = path[currentIndex];
            direction = target - transform.position;
        }

        // Move the car
        transform.position += direction.normalized * currentSpeed * Time.deltaTime;

        // Rotate car in 2D
        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
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
                {
                    closestDistance = dist;
                }
                carDetected = true;
            }
        }

        if (carDetected)
        {
            if (closestDistance < stopDistance)
            {
                currentSpeed = 0f;
            }
            else
            {
                float t = (closestDistance - stopDistance) / (detectionRadius - stopDistance);
                currentSpeed = Mathf.Lerp(0f, maxSpeed, t);
            }
        }
        else
        {
            currentSpeed = maxSpeed;
        }
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
