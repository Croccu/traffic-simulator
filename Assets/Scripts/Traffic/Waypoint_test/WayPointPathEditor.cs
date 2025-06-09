using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointPath))]
public class WayPointPathEditor : Editor
{
    void OnSceneGUI()
    {
        WaypointPath path = (WaypointPath)target;

        if (path.controlPoints.Count == 4)
        {
            Handles.color = Color.green;
            Handles.DrawBezier(
                path.controlPoints[0].position,
                path.controlPoints[3].position,
                path.controlPoints[1].position,
                path.controlPoints[2].position,
                Color.yellow,
                null,
                2f
            );

            for (int i = 0; i < path.controlPoints.Count; i++)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(path.controlPoints[i].position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(path.controlPoints[i], "Move Handle");
                    path.controlPoints[i].position = newPos;
                }
            }

            if (Handles.Button(path.transform.position + Vector3.up * 2f, Quaternion.identity, 0.5f, 0.5f, Handles.SphereHandleCap))
            {
                path.GenerateWaypoints();
            }
        }
    }
}
