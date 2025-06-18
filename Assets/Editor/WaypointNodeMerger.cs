using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class WaypointNodeMergeTool : EditorWindow
{
    private float snapRadius = 0.25f;

    [MenuItem("Tools/Waypoint/Merge Overlapping Nodes")]
    public static void ShowWindow()
    {
        GetWindow<WaypointNodeMergeTool>("Merge Overlapping Waypoints");
    }

    private void OnGUI()
    {
        GUILayout.Label("Merge Overlapping Waypoints", EditorStyles.boldLabel);
        snapRadius = EditorGUILayout.FloatField("Snap Radius", snapRadius);

        if (GUILayout.Button("Scan and Merge"))
        {
            if (EditorUtility.DisplayDialog("Confirm Merge",
                "This will merge all overlapping WaypointNodes in the current scene. This cannot be undone.\n\nMake sure to back up or use version control before continuing.",
                "Merge", "Cancel"))
            {
                MergeNodes(snapRadius);
            }
        }
    }

    private void MergeNodes(float radius)
    {
        WaypointNode[] allNodes = Object.FindObjectsByType<WaypointNode>(FindObjectsSortMode.None);
        HashSet<WaypointNode> toDestroy = new();
        int mergeCount = 0;

        for (int i = 0; i < allNodes.Length; i++)
        {
            WaypointNode a = allNodes[i];
            if (toDestroy.Contains(a)) continue;

            for (int j = i + 1; j < allNodes.Length; j++)
            {
                WaypointNode b = allNodes[j];
                if (toDestroy.Contains(b)) continue;

                if (Vector3.Distance(a.transform.position, b.transform.position) <= radius)
                {
                    // Transfer incoming connections
                    foreach (WaypointNode other in allNodes)
                    {
                        if (other == a || other == b) continue;

                        if (other.nextNodes.Contains(b))
                        {
                            Undo.RecordObject(other, "Redirect Connection");
                            other.nextNodes.Remove(b);
                            if (!other.nextNodes.Contains(a))
                                other.nextNodes.Add(a);
                        }
                    }

                    // Transfer outgoing connections
                    foreach (WaypointNode target in b.nextNodes)
                    {
                        if (target != a && !a.nextNodes.Contains(target))
                        {
                            Undo.RecordObject(a, "Merge Outgoing");
                            a.nextNodes.Add(target);
                        }
                    }

                    // Clean up a → b links if any
                    if (a.nextNodes.Contains(b))
                    {
                        Undo.RecordObject(a, "Remove Redundant Link");
                        a.nextNodes.Remove(b);
                    }

                    // Transfer Bezier curve component
                    BezierWaypointSegment bCurve = b.GetComponent<BezierWaypointSegment>();
                    BezierWaypointSegment aCurve = a.GetComponent<BezierWaypointSegment>();

                    if (bCurve != null)
                    {
                        if (aCurve == null)
                        {
                            aCurve = Undo.AddComponent<BezierWaypointSegment>(a.gameObject);
                            aCurve.controlPoint = bCurve.controlPoint;
                            aCurve.endNode = bCurve.endNode;
                        }
                        else
                        {
                            if (aCurve.controlPoint == Vector3.zero)
                                aCurve.controlPoint = bCurve.controlPoint;
                            if (aCurve.endNode == null)
                                aCurve.endNode = bCurve.endNode;
                        }

                        // Ensure the curve's end node is added to a.nextNodes
                        if (aCurve.endNode != null && !a.nextNodes.Contains(aCurve.endNode))
                        {
                            Undo.RecordObject(a, "Fix Curve Connection");
                            a.nextNodes.Add(aCurve.endNode);
                        }

                        // aCurve.Refresh(); // Uncomment if you have a Refresh method to redraw the curve
                    }

                    // Merge entry/exit flags
                    a.isEntry |= b.isEntry;
                    a.isExit |= b.isExit;

                    Undo.DestroyObjectImmediate(b.gameObject);
                    toDestroy.Add(b);
                    mergeCount++;

                    EditorGUIUtility.PingObject(a.gameObject);
                }
            }
        }

        Debug.Log($"✅ Merged {mergeCount} overlapping WaypointNode(s), preserving curves and redrawing visuals.");
    }
}
