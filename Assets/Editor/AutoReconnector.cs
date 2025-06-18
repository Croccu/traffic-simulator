using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

[InitializeOnLoad]
public static class AutoReconnectOnPlay
{
  static AutoReconnectOnPlay()
  {
    Debug.Log("AutoReconnectOnPlay initialized 🚀");
    EditorApplication.playModeStateChanged += OnPlayModeChanged;
  }

  private static void OnPlayModeChanged(PlayModeStateChange change)
  {
    if (change == PlayModeStateChange.ExitingEditMode)
    {
      Debug.Log("🔄 Auto-Reconnecting waypoint segments before Play Mode...");
      ReconnectAllSegments(0.25f); // You can adjust snap radius here
    }
  }

  private static void ReconnectAllSegments(float snapRadius)
  {
    WaypointNode[] allNodes = Object.FindObjectsByType<WaypointNode>(FindObjectsSortMode.None);
    BezierWaypointSegment[] allSegments = Object.FindObjectsByType<BezierWaypointSegment>(FindObjectsSortMode.None);

    foreach (WaypointNode node in allNodes)
    {
      node.outgoingCurves.Clear();
      node.nextNodes.Clear();
    }

    int reconnected = 0;

    foreach (BezierWaypointSegment segment in allSegments)
    {
      if (segment == null || segment.endNode == null) continue;

      float closestDist = float.MaxValue;
      WaypointNode closest = null;

      foreach (WaypointNode node in allNodes)
      {
        float dist = Vector3.Distance(node.transform.position, segment.transform.position);
        if (dist < closestDist && dist <= snapRadius)
        {
          closestDist = dist;
          closest = node;
        }
      }

      if (closest != null)
      {
        closest.outgoingCurves.Add(segment);
        if (!closest.nextNodes.Contains(segment.endNode))
          closest.nextNodes.Add(segment.endNode);
        reconnected++;
      }
    }

    Debug.Log($"✅ Auto-reconnected {reconnected} segments before Play.");
  }
}
