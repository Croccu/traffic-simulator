using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class WaypointReconnectorTool : EditorWindow
{
    private float snapRadius = 0.25f;

    [MenuItem("Tools/Waypoint/Reconnect All Curves")]
    public static void ShowWindow()
    {
        GetWindow<WaypointReconnectorTool>("Reconnect Waypoints");
    }

    private void OnGUI()
    {
        GUILayout.Label("Reconnect Bezier Segments to Waypoint Nodes", EditorStyles.boldLabel);
        snapRadius = EditorGUILayout.FloatField("Snap Radius", snapRadius);

        if (GUILayout.Button("Reconnect Now"))
        {
            if (EditorUtility.DisplayDialog("Confirm Reconnection",
                "This will scan all BezierWaypointSegments in the scene and attempt to reconnect them to nearby WaypointNodes.\n\nMake sure your scene is saved!",
                "Run", "Cancel"))
            {
                ReconnectAllSegments();
            }
        }
    }

    private void ReconnectAllSegments()
    {
        WaypointNode[] allNodes = Object.FindObjectsByType<WaypointNode>(FindObjectsSortMode.None);
        BezierWaypointSegment[] allSegments = Object.FindObjectsByType<BezierWaypointSegment>(FindObjectsSortMode.None);

        // Reset
        foreach (WaypointNode node in allNodes)
        {
            Undo.RecordObject(node, "Clear Curves");
            node.outgoingCurves.Clear();
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
                Undo.RecordObject(closest, "Reconnect Segment");
                closest.outgoingCurves.Add(segment);

                // Optional: repair nextNodes too
                if (!closest.nextNodes.Contains(segment.endNode))
                    closest.nextNodes.Add(segment.endNode);

                reconnected++;
            }
        }

        Debug.Log($"✅ Reconnected {reconnected} Bezier segments to WaypointNodes.");
    }
}
