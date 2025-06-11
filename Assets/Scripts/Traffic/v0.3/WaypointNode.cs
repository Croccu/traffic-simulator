using UnityEngine;
using System.Collections.Generic;

public class WaypointNode : MonoBehaviour
{
  public List<WaypointNode> connectedNodes = new();
  public bool isEntry = false;
  public bool isExit = false;

  void OnDrawGizmos()
  {
    Gizmos.color = isExit ? Color.red : isEntry ? Color.green : Color.cyan;
    Gizmos.DrawSphere(transform.position, 0.1f);

    Gizmos.color = Color.yellow;
    foreach (var node in connectedNodes)
    {
      if (node != null)
        Gizmos.DrawLine(transform.position, node.transform.position);
    }
  }
}
