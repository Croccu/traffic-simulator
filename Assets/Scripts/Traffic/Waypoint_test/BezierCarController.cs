using System.Collections.Generic;
using UnityEngine;

public class BezierCarController : MonoBehaviour
{
    private List<Vector3> path;
    private int currentIndex = 0;

    [Header("Car Settings")]
    public float speed = 5f;
    public float rotationSpeed = 5f;

    private bool isMoving = false;

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
    }

    void Update()
    {
        if (!isMoving || path == null || currentIndex >= path.Count)
            return;

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
        transform.position += direction.normalized * speed * Time.deltaTime;

        // Rotate the car smoothly
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
