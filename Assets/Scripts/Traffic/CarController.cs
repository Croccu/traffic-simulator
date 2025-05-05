using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
  public float speed = 3f;
  private float originalSpeed;

  [Header("Pathfinding")]
  public List<Waypoint> path = new List<Waypoint>();
  private int pathIndex = 0;

  [Header("Collision Avoidance")]
  public float detectionRadius = 0.5f;
  public LayerMask carLayer;

  private bool isWaitingAtStopLine = false; // Flag to track if the car is waiting at a StopLine

  private void Start()
  {
    originalSpeed = speed; // Store the original speed
  }

  private void Update()
  {
    // Stop if no path or path is complete
    if (path == null || pathIndex >= path.Count) return;

    // If the car is waiting at a StopLine, do not proceed
    if (isWaitingAtStopLine) return;

    // Check for cars in front
    if (IsCarInFront())
    {
      StopCar("Car in front detected.");
      return;
    }

    // Resume movement if no car is in front and speed is 0
    if (speed == 0)
    {
      RestoreSpeed("No car in front, resuming movement.");
    }

    MoveTowardsWaypoint();
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("StopLine"))
    {
      StartCoroutine(StopAndWait());
    }
  }

  private IEnumerator StopAndWait()
  {
    isWaitingAtStopLine = true; // Set the flag to prevent movement
    StopCar("Stopped at StopLine.");

    yield return new WaitForSeconds(2f); // Wait for 3 seconds
    RestoreSpeed("Resumed after StopLine.");

    isWaitingAtStopLine = false; // Clear the flag to allow movement
  }

  private bool IsCarInFront()
  {
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, carLayer);
    foreach (var hit in hits)
    {
      if (hit.gameObject != gameObject)
      {
        Vector3 directionToCar = hit.transform.position - transform.position;
        if (Vector3.Angle(transform.up, directionToCar) < 45f)
        {
          return true; // Another car is in front
        }
      }
    }
    return false;
  }

  private void MoveTowardsWaypoint()
  {
    Waypoint target = path[pathIndex];
    Vector3 direction = target.transform.position - transform.position;

    // Move towards the target waypoint
    transform.position += direction.normalized * speed * Time.deltaTime;

    // Rotate the car to face the direction it is moving
    if (direction.sqrMagnitude > 0.01f)
    {
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // Move to the next waypoint if close enough
    if (direction.magnitude < 0.1f)
    {
      pathIndex++;
    }
  }

  private void StopCar(string reason)
  {
    speed = 0;
    Debug.Log($"Car {gameObject.name} stopped: {reason}");
  }

  private void RestoreSpeed(string reason)
  {
    speed = originalSpeed;
    Debug.Log($"Car {gameObject.name} restored speed: {reason}");
  }
}