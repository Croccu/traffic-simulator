using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaypointNode : MonoBehaviour
{
  public List<WaypointNode> connectedNodes = new();
  public bool isEntry = false;
  public bool isExit = false;

  void OnDrawGizmos()
  {
    if (connectedNodes.Count == 0 && !isEntry && !isExit)
      Gizmos.color = new Color(1f, 0.5f, 0f); // orange for unconnected
    else if (isExit)
      Gizmos.color = Color.red;
    else if (isEntry)
      Gizmos.color = Color.green;
    else
      Gizmos.color = Color.cyan;

    Gizmos.DrawSphere(transform.position, 0.1f);

    Gizmos.color = Color.yellow;
    foreach (var node in connectedNodes)
    {
      if (node != null)
        Gizmos.DrawLine(transform.position, node.transform.position);
    }

    #if UNITY_EDITOR
    int incomingCount = 0;
    foreach (WaypointNode node in FindObjectsByType<WaypointNode>(FindObjectsSortMode.None))
    {
      if (node != this && node.connectedNodes.Contains(this))
        incomingCount++;
    }

    if (incomingCount >= 2)
    {
      GUIStyle style = new GUIStyle();
      style.normal.textColor = Color.white;
      style.fontSize = 10;
      Handles.Label(transform.position + Vector3.up * 0.4f, incomingCount.ToString(), style);
    }
    #endif
  }
}
