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
  public float detectionRadius = 0.5f; // Default detection radius
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
    if (IsCarInFront(45f)) // Default narrow angle for cars in front
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

    // Temporarily increase detection radius for wider range
    float originalDetectionRadius = detectionRadius;
    detectionRadius = 1.5f; // Increase detection radius while waiting

    // Wait for 1 second while checking for cars in a wider angle
    float waitTime = 1f; // Total wait time
    float elapsedTime = 0f; // Time elapsed

    while (elapsedTime < waitTime)
    {
      if (IsCarInFrontWithBias(180f)) // Adjusted detection logic for left bias
      {
        Debug.Log("Car detected while waiting at StopLine.");
        elapsedTime += Time.deltaTime * 0.5f; // Slow increment when a car is detected
      }
      else
      {
        elapsedTime += Time.deltaTime; // Normal increment when no car is detected
      }
      yield return null;
    }

    // Gradually reduce detection radius to simulate driver readiness to go
    while (IsCarInFrontWithBias(180f) && detectionRadius > originalDetectionRadius * 0.5f)
    {
      detectionRadius -= Time.deltaTime * 0.1f; // Gradually reduce detection radius
      Debug.Log($"Reducing detection radius: {detectionRadius}");
      yield return null;
    }

    // Restore original detection radius
    detectionRadius = originalDetectionRadius;

    RestoreSpeed("Resumed after StopLine.");
    isWaitingAtStopLine = false; // Clear the flag to allow movement
  }

  private bool IsCarInFrontWithBias(float angleThreshold)
  {
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, carLayer);
    foreach (var hit in hits)
    {
      if (hit.gameObject != gameObject)
      {
        Vector3 directionToCar = hit.transform.position - transform.position;
        float angle = Vector3.Angle(transform.up, directionToCar);

        // Adjust detection logic for left bias
        if (angle < angleThreshold)
        {
          // Prioritize cars on the left
          if (directionToCar.x < 0) // Car is on the left
          {
            return true;
          }

          // Deprioritize cars on the right
          if (directionToCar.x > 0 && angle < angleThreshold * 0.5f) // Narrower angle for the right
          {
            return true;
          }
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

  private bool IsCarInFront(float angleThreshold)
  {
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, carLayer);
    foreach (var hit in hits)
    {
      if (hit.gameObject != gameObject)
      {
        Vector3 directionToCar = hit.transform.position - transform.position;
        float angle = Vector3.Angle(transform.up, directionToCar);

        if (angle < angleThreshold)
        {
          return true;
        }
      }
    }
    return false;
  }
}