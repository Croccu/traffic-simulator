using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaypointNode : MonoBehaviour
{
    public List<WaypointNode> nextNodes = new();
    public bool isEntry = false;
    public bool isExit = false;

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

        // nextNodes (green bezier or straight lines)
        Gizmos.color = Color.green;
        foreach (WaypointNode next in nextNodes)
        {
            if (next == null) continue;

            BezierWaypointSegment bezier = GetComponent<BezierWaypointSegment>();
            if (bezier != null && bezier.endNode == next)
            {
                Vector3 p0 = transform.position;
                Vector3 p1 = bezier.controlPoint;
                Vector3 p2 = next.transform.position;

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
            else
            {
                Gizmos.DrawLine(transform.position, next.transform.position);
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

            // Outgoing count (cyan, right)
            if (nextNodes.Count >= 2)
            {
                GUIStyle outStyle = new GUIStyle
                {
                    normal = { textColor = Color.cyan },
                    fontSize = 10
                };
                Handles.Label(transform.position + Vector3.up * 0.4f + Vector3.right * 0.2f, $"→{nextNodes.Count}", outStyle);
            }
        }
#endif
    }

    void LateUpdate()
    {
        drawnLabels.Clear();
    }
}
