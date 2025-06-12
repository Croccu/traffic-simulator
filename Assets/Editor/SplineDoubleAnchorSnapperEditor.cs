using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineDoubleAnchorSnapper))]
public class SplineDoubleAnchorSnapperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SplineDoubleAnchorSnapper snapper = (SplineDoubleAnchorSnapper)target;

        if (GUILayout.Button("Snap Now"))
        {
            snapper.Snap();
        }
    }
}