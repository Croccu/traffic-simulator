using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineAnchorSnapper))]
public class SplineAnchorSnapperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SplineAnchorSnapper snapper = (SplineAnchorSnapper)target;

        if (GUILayout.Button("Snap Now"))
        {
            snapper.Snap();
        }
    }
}