using UnityEngine;
using System.Collections.Generic;

public class WaypointRoute : MonoBehaviour
{
  [Tooltip("List of waypoints in order for cars to follow.")]
  public List<WaypointNode> nodes = new();

  void OnDrawGizmos()
  {
    if (nodes == null || nodes.Count < 2) return;

    Gizmos.color = Color.cyan;
    for (int i = 0; i < nodes.Count - 1; i++)
    {
      if (nodes[i] != null && nodes[i + 1] != null)
      {
        Gizmos.DrawLine(nodes[i].transform.position, nodes[i + 1].transform.position);
      }
    }
  }
}
