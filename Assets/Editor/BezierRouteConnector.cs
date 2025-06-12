using UnityEditor;
using UnityEngine;

public class BezierRouteConnector : EditorWindow
{
    private BezierRouteSpline splineA;
    private BezierRouteSpline splineB;

    [MenuItem("Tools/Bezier Spline Connector")]
    public static void ShowWindow()
    {
        GetWindow<BezierRouteConnector>("Spline Connector");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Select Splines to Connect", EditorStyles.boldLabel);
        splineA = (BezierRouteSpline)EditorGUILayout.ObjectField("Spline A", splineA, typeof(BezierRouteSpline), true);
        splineB = (BezierRouteSpline)EditorGUILayout.ObjectField("Spline B", splineB, typeof(BezierRouteSpline), true);

        EditorGUILayout.Space();

        GUI.enabled = splineA != null && splineB != null && splineA != splineB;
        if (GUILayout.Button("Connect Spline B into A"))
        {
            splineA.ConnectTo(splineB);
        }
        GUI.enabled = true;
    }

    void OnFocus()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (splineA != null && splineB != null)
        {
            if (splineA.controlPoints.Count > 0 && splineB.controlPoints.Count > 0)
            {
                Vector3 endA = splineA.controlPoints[splineA.controlPoints.Count - 1].position;
                Vector3 startB = splineB.controlPoints[0].position;

                Handles.color = Color.cyan;
                Handles.DrawDottedLine(endA, startB, 4f);
                Handles.Label((endA + startB) / 2, "Will connect");
            }
        }
    }
}
