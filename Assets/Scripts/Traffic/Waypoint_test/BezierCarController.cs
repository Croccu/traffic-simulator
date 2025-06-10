using System.Collections.Generic;
using UnityEngine;

public class BezierCarController : MonoBehaviour
{
    private List<Vector3> path;
    private int currentIndex = 0;

    [Header("Car Settings")]
    public float maxSpeed = 5f;
    public float rotationSpeed = 5f;
    public float detectionDistance = 2f;
    public float slowDownDistance = 1f;

    private float currentSpeed;
    private bool isMoving = false;

    public LayerMask carLayer;

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
        currentSpeed = maxSpeed;
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving || path == null || currentIndex >= path.Count)
            return;

        Vector3 target = path[currentIndex];
        Vector3 direction = target - transform.position;
        direction.z = 0;

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
            direction.z = 0;
        }

        // Default speed
        float speed = maxSpeed;

        // Check for car ahead
        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, detectionDistance, carLayer))
        {
            BezierCarController otherCar = hit.collider.GetComponent<BezierCarController>();
            if (otherCar != null)
            {
                float distance = hit.distance;

                if (!otherCar.isMoving || otherCar.currentSpeed < 0.1f)
                {
                    speed = 0f; // stop
                }
                else if (distance < slowDownDistance)
                {
                    speed = maxSpeed * 0.5f; // slow down
                }
            }
        }

        // Move the car
        transform.position += direction.normalized * speed * Time.deltaTime;
        currentSpeed = speed;

        // Rotate the car
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (path != null && currentIndex < path.Count)
        {
            Vector3 dir = path[currentIndex] - transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + dir.normalized * detectionDistance);
        }
    }
}
