using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierRouteSpline))]
public class BezierRouteAnchorEditor : Editor
{
    // Connection threshold
    private const float connectThreshold = 0.5f;
    private const float proximityRange = 2.0f;

    void OnSceneGUI()
    {
        BezierRouteSpline[] allSplines = FindObjectsOfType<BezierRouteSpline>();

        foreach (var splineA in allSplines)
        {
            if (splineA.controlPoints == null || splineA.controlPoints.Count == 0)
                continue;

            Transform endA = splineA.controlPoints[splineA.controlPoints.Count - 1];
            if (endA == null) continue;

            foreach (var splineB in allSplines)
            {
                if (splineA == splineB || splineB.controlPoints == null || splineB.controlPoints.Count == 0)
                    continue;

                Transform startB = splineB.controlPoints[0];
                if (startB == null) continue;

                float distance = Vector3.Distance(endA.position, startB.position);

                // Close enough to connect
                if (distance < connectThreshold)
                {
                    if (!splineA.nextSplines.Contains(splineB))
                    {
                        Undo.RecordObject(splineA, "Connect Splines");
                        splineA.ConnectTo(splineB);
                        EditorUtility.SetDirty(splineA);
                    }

                    Handles.color = Color.green;
                    Handles.DrawLine(endA.position, startB.position);
                }
                // Close but not close enough: Show red
                else if (distance < proximityRange)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(endA.position, startB.position);
                }
            }
        }
    }
}
