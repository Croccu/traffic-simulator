using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class WaypointGraphRestorer : MonoBehaviour
{
    [Tooltip("Max distance to snap a segment to a node.")]
    public float snapRadius = 0.3f;

    private void Awake()
    {
#if UNITY_EDITOR
        ReconnectAllSegments();
#endif
    }

    private void ReconnectAllSegments()
    {
        WaypointNode[] allNodes = Object.FindObjectsByType<WaypointNode>(FindObjectsSortMode.None);
        BezierWaypointSegment[] allSegments = Object.FindObjectsByType<BezierWaypointSegment>(FindObjectsSortMode.None);


        foreach (var node in allNodes)
        {
            node.outgoingCurves.Clear();
            node.nextNodes.Clear();
        }

        int reconnected = 0;

        foreach (var segment in allSegments)
        {
            if (segment == null || segment.endNode == null) continue;

            float closestDist = float.MaxValue;
            WaypointNode closest = null;

            foreach (var node in allNodes)
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

        Debug.Log($"WaypointGraphRestorer: Reconnected {reconnected} segment(s).");
    }
}
