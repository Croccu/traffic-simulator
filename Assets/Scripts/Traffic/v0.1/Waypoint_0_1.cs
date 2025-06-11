using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
  [Header("Graph Connections")]
  public List<Waypoint> connectedWaypoints = new List<Waypoint>();

  [Header("Optional - Debug")]
  public bool showConnections = true;

  private void OnDrawGizmos()
  {
    if (!showConnections || connectedWaypoints == null) return;

    Gizmos.color = Color.cyan;

    foreach (var neighbor in connectedWaypoints)
    {
      if (neighbor != null)
      {
        Gizmos.DrawLine(transform.position, neighbor.transform.position);
        Gizmos.DrawSphere(neighbor.transform.position, 0.05f);
      }
    }

    Gizmos.color = Color.green;
    Gizmos.DrawSphere(transform.position, 0.07f); // draw self too
  }
}
