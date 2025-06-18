using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaypointNode : MonoBehaviour
{
    [SerializeField] public List<WaypointNode> nextNodes = new(); // Still used for logic
    public bool isEntry = false;
    public bool isExit = false;

    [SerializeField] public List<BezierWaypointSegment> outgoingCurves = new();

    private static HashSet<WaypointNode> drawnLabels = new();

    void OnDrawGizmos()
    {
        // Node coloring
        if (!isEntry && !isExit && nextNodes.Count == 0)
            Gizmos.color = new Color(1f, 0.5f, 0f); // orange = isolated
        else if (isExit)
            Gizmos.color = Color.red;
        else if (isEntry)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.cyan;

        Gizmos.DrawSphere(transform.position, 0.1f);

        // ✅ Draw bezier curves from outgoingCurves
        foreach (BezierWaypointSegment segment in outgoingCurves)
        {
            if (segment == null || segment.endNode == null) continue;

            Vector3 p0 = transform.position;
            Vector3 p1 = segment.controlPoint;
            Vector3 p2 = segment.endNode.transform.position;

            Gizmos.color = Color.green;
            Vector3 prev = p0;
            int segments = 20;
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 pt = Mathf.Pow(1 - t, 2) * p0 +
                             2 * (1 - t) * t * p1 +
                             Mathf.Pow(t, 2) * p2;
                Gizmos.DrawLine(prev, pt);
                prev = pt;
            }
        }

#if UNITY_EDITOR
        if (!drawnLabels.Contains(this) && !PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            drawnLabels.Add(this);

            int incomingCount = 0;
            foreach (WaypointNode node in FindObjectsByType<WaypointNode>(FindObjectsSortMode.None))
            {
                if (node != this && node.nextNodes.Contains(this))
                    incomingCount++;
            }

            // Incoming count (white, left)
            if (incomingCount >= 2)
            {
                GUIStyle style = new GUIStyle
                {
                    normal = { textColor = Color.white },
                    fontSize = 10
                };
                Handles.Label(transform.position + Vector3.up * 0.4f + Vector3.left * 0.2f, incomingCount.ToString(), style);
            }

            // Outgoing count (cyan, right) now reflects curve count
            if (outgoingCurves.Count >= 2)
            {
                GUIStyle outStyle = new GUIStyle
                {
                    normal = { textColor = Color.cyan },
                    fontSize = 10
                };
                Handles.Label(transform.position + Vector3.up * 0.4f + Vector3.right * 0.2f, $"→{outgoingCurves.Count}", outStyle);
            }
        }
#endif
    }

    void LateUpdate()
    {
        drawnLabels.Clear();
    }
}
