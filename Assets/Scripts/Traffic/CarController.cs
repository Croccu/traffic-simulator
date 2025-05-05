using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
  public float speed = 2f;
  private float originalSpeed;

  [Header("Pathfinding")]
  public List<Waypoint> path = new List<Waypoint>();
  private int pathIndex = 0;

  private bool isStopped = false;

  // Global stop flag
  public static bool globalStop = false;
  private bool wasGloballyStopped = false; // Track the previous state of globalStop

  [Header("Collision Avoidance")]
  public float detectionRadius = 1.5f;
  public LayerMask carLayer;

  private void Start()
  {
    originalSpeed = speed; // Store the original speed
  }

  private void Update()
  {
    // Check if the global stop flag has been toggled off
    if (wasGloballyStopped && !globalStop)
    {
      // Recalculate the path when resuming from a global stop
      RecalculatePath();
      wasGloballyStopped = false; // Reset the flag
      isStopped = false; // Allow the car to move again
    }

    // Update the global stop tracking flag
    if (globalStop)
    {
      wasGloballyStopped = true;
      isStopped = true; // Stop the car globally
    }

    // Stop the car if globally stopped or if it is manually stopped
    if (path == null || pathIndex >= path.Count || isStopped) return;

    // Check for nearby cars to avoid collisions
    if (IsCarInFront())
    {
      Debug.Log($"Car {gameObject.name} is stopping to avoid collision.");
      return; // Stop moving if another car is in front
    }

    Waypoint target = path[pathIndex];
    Vector3 direction = target.transform.position - transform.position;

    // Move towards current target waypoint
    transform.position += direction.normalized * speed * Time.deltaTime;

    // Rotate the car to face the direction it is moving
    if (direction.sqrMagnitude > 0.01f)
    {
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // If we're close enough, move to the next waypoint
    if (direction.magnitude < 0.1f)
    {
      pathIndex++;
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Intersection"))
    {
      if (IntersectionControl.stopAtIntersection)
      {
        // Stop the car if the global flag is active
        isStopped = true;
        Debug.Log($"Car {gameObject.name} stopped at the intersection due to control.");
      }
      else
      {
        // Slow down the car when entering the intersection
        speed = originalSpeed * 0.5f;
        Debug.Log($"Car {gameObject.name} is slowing down at the intersection.");
      }
    }
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("Intersection"))
    {
      // Restore the car's speed and allow it to move again
      speed = originalSpeed;
      isStopped = false;
      Debug.Log($"Car {gameObject.name} has exited the intersection and restored speed.");
    }
  }

  private bool IsCarInFront()
  {
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, carLayer);
    foreach (var hit in hits)
    {
      if (hit.gameObject != gameObject)
      {
        Vector3 directionToCar = hit.transform.position - transform.position;
        float angle = Vector3.Angle(transform.up, directionToCar);

        if (angle < 45f)
        {
          return true; // Another car is in front
        }
      }
    }
    return false;
  }

  private void RecalculatePath()
  {
    if (path == null || path.Count == 0) return;

    // Get the current target waypoint
    Waypoint currentTarget = path[pathIndex];

    // Recalculate the path from the car's current position to the target
    Waypoint currentWaypoint = FindClosestWaypoint();
    if (currentWaypoint != null)
    {
      path = Pathfinding.FindPath(currentWaypoint, currentTarget);
      pathIndex = 0; // Reset path index
      Debug.Log($"Car {gameObject.name} recalculated its path.");
    }
  }

  private Waypoint FindClosestWaypoint()
  {
    // Find the closest waypoint to the car's current position
    Waypoint closest = null;
    float minDistance = Mathf.Infinity;

    foreach (Waypoint waypoint in FindObjectsByType<Waypoint>(FindObjectsSortMode.None))
    {
      float distance = Vector3.Distance(transform.position, waypoint.transform.position);
      if (distance < minDistance)
      {
        minDistance = distance;
        closest = waypoint;
      }
    }

    return closest;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, detectionRadius);
  }
}