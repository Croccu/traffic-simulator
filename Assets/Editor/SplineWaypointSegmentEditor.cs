using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineWaypointSegment))]
public class SplineWaypointSegmentEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    SplineWaypointSegment generator = (SplineWaypointSegment)target;

    if (GUILayout.Button("Generate Waypoints"))
    {
      generator.GenerateWaypoints();
    }
  }
}
