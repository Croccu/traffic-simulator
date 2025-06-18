using UnityEngine;

[ExecuteInEditMode]
public class BezierWaypointSegment : MonoBehaviour
{
    public WaypointNode endNode;
    public Vector3 controlPoint;

    // private void OnDrawGizmos()
    // {
    //     if (endNode == null) return;

    //     Gizmos.color = Color.green;

    //     Vector3 p0 = transform.position;
    //     Vector3 p1 = controlPoint;
    //     Vector3 p2 = endNode.transform.position;

    //     Vector3 prev = p0;
    //     for (float t = 0; t <= 1f; t += 0.05f)
    //     {
    //         Vector3 pt = Mathf.Pow(1 - t, 2) * p0 +
    //                      2 * (1 - t) * t * p1 +
    //                      Mathf.Pow(t, 2) * p2;
    //         Gizmos.DrawLine(prev, pt);
    //         prev = pt;
    //     }
    // }

    // // Call this after any edits to refresh the gizmo
    // public void Refresh()
    // {
    //     #if UNITY_EDITOR
    //     UnityEditor.EditorUtility.SetDirty(this);
    //     UnityEditor.SceneView.RepaintAll();
    //     #endif
    // }
}
