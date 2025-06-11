using UnityEngine;
using System.Collections.Generic;

public class SplineCarController : MonoBehaviour
{
  public float speed = 5f;
  public float rotationSpeed = 5f;

  private List<WaypointNode> path = new();
  private int currentIndex = 0;
  private bool isMoving = false;

  public void SetPath(List<WaypointNode> nodes)
  {
    if (nodes == null || nodes.Count < 2) return;

    path = nodes;
    transform.position = path[0].transform.position;
    currentIndex = 1;
    isMoving = true;
  }

  void Update()
  {
    if (!isMoving || currentIndex >= path.Count) return;

    Vector3 target = path[currentIndex].transform.position;
    Vector3 dir = target - transform.position;

    if (dir.magnitude < 0.1f)
    {
      if (path[currentIndex].isStopPoint)
      {
        isMoving = false;
        return;
      }

      currentIndex++;
      if (currentIndex >= path.Count)
      {
        Destroy(gameObject);
        return;
      }

      target = path[currentIndex].transform.position;
      dir = target - transform.position;
    }

    transform.position += dir.normalized * speed * Time.deltaTime;

    if (dir.sqrMagnitude > 0.01f)
    {
      Quaternion lookRot = Quaternion.LookRotation(Vector3.forward, dir.normalized);
      transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
    }
  }
}
