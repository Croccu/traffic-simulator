using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CurvedWaypointPlacer))]
public class CurvedWaypointPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CurvedWaypointPlacer placer = (CurvedWaypointPlacer)target;

        // DrawDefaultInspector(); disable this to prevent duplication

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Naming Settings", EditorStyles.boldLabel);

        placer.waypointPrefix = EditorGUILayout.TextField("Prefix", placer.waypointPrefix);
        placer.namingIndex = EditorGUILayout.IntField("Start Index", placer.namingIndex);
        placer.useDoubleSuffix = EditorGUILayout.Toggle("Use Double Suffix (x.x)", placer.useDoubleSuffix);

        if (GUILayout.Button("Reset Index to 1"))
        {
            placer.namingIndex = 1;
        }

        if (GUI.changed)
            EditorUtility.SetDirty(placer);
    }
}
