using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
  public float speed = 2f;

  [Header("Pathfinding")]
  public List<Waypoint> path = new List<Waypoint>();
  private int pathIndex = 0;

  private void Update()
  {
    if (path == null || pathIndex >= path.Count) return;

    Waypoint target = path[pathIndex];
    Vector3 direction = target.transform.position - transform.position;

    // Move towards current target waypoint
    transform.position += direction.normalized * speed * Time.deltaTime;

    // Rotate the car to face the direction it is moving
    if (direction.sqrMagnitude > 0.01f) // Avoid rotating if the direction is too small
    {
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // Subtract 90 degrees to align with Y-axis
    }

    // If we're close enough, move to the next waypoint
    if (direction.magnitude < 0.1f)
    {
      pathIndex++;
    }
  }
}