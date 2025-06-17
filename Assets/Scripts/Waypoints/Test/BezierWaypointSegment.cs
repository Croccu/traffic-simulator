using UnityEngine;

public class BezierWaypointSegment : MonoBehaviour
{
    public Vector3 controlPoint;
    public WaypointNode endNode;

    public Vector3 GetPoint(float t)
    {
        Vector3 p0 = transform.position;
        Vector3 p1 = controlPoint;
        Vector3 p2 = endNode.transform.position;

        return Mathf.Pow(1 - t, 2) * p0 +
               2 * (1 - t) * t * p1 +
               Mathf.Pow(t, 2) * p2;
    }

}
