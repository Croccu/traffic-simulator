using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarAgent : MonoBehaviour
{
  public float speed = 5f;
  public float turnSpeed = 5f;

  private List<WaypointNode> path;
  private int currentIndex = 0;
  private bool isMoving = false;

  void Start()
  {
    StartCoroutine(Initialize());
  }

  IEnumerator Initialize()
  {
    yield return new WaitForSeconds(0.2f);

    WaypointNode start = WaypointGraph.Instance.GetRandomEntry();
    WaypointNode goal = WaypointGraph.Instance.GetRandomExit();

    if (start == null || goal == null || start == goal) yield break;

    path = FindPath(start, goal);
    if (path == null || path.Count == 0) yield break;

    transform.position = path[0].transform.position;
    currentIndex = 1;
    isMoving = true;
  }

  void Update()
  {
    if (!isMoving || path == null || currentIndex >= path.Count) return;

    Vector3 target = path[currentIndex].transform.position;
    Vector3 dir = target - transform.position;

    if (dir.magnitude < 0.1f)
    {
      currentIndex++;
      if (currentIndex >= path.Count)
      {
        Destroy(gameObject);
        return;
      }

      target = path[currentIndex].transform.position;
    }

    transform.position += dir.normalized * speed * Time.deltaTime;

    if (dir.sqrMagnitude > 0.01f)
    {
      Quaternion lookRot = Quaternion.LookRotation(Vector3.forward, dir.normalized);
      transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, turnSpeed * Time.deltaTime);
    }
  }

  List<WaypointNode> FindPath(WaypointNode start, WaypointNode goal)
  {
    Queue<List<WaypointNode>> queue = new();
    HashSet<WaypointNode> visited = new();
    queue.Enqueue(new List<WaypointNode> { start });

    while (queue.Count > 0)
    {
      var currentPath = queue.Dequeue();
      WaypointNode current = currentPath[^1];

      if (visited.Contains(current)) continue;
      visited.Add(current);

      if (current == goal) return currentPath;

      foreach (var neighbor in current.nextNodes)
      {
        if (neighbor == null || visited.Contains(neighbor)) continue;
        var newPath = new List<WaypointNode>(currentPath) { neighbor };
        queue.Enqueue(newPath);
      }
    }

    return null;
  }
}
