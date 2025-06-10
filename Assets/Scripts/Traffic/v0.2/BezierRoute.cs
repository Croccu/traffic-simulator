using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BezierRoute : MonoBehaviour
{
    public string routeName = "Route_1";

    public Transform p0, p1, p2, p3;
    [Range(2, 100)] public int resolution = 20;
    public List<Vector3> waypoints = new List<Vector3>();

    public void GenerateWaypoints()
    {
        waypoints.Clear();
        if (!p0 || !p1 || !p2 || !p3) return;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = CalculateBezier(t, p0.position, p1.position, p2.position, p3.position);
            waypoints.Add(point);
        }
    }

    Vector3 CalculateBezier(float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        return Mathf.Pow(1 - t, 3) * a +
               3 * Mathf.Pow(1 - t, 2) * t * b +
               3 * (1 - t) * Mathf.Pow(t, 2) * c +
               Mathf.Pow(t, 3) * d;
    }

    private void OnDrawGizmos()
    {
        if (!p0 || !p1 || !p2 || !p3) return;

        GenerateWaypoints();

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(p0.position, p1.position);
        Gizmos.DrawLine(p2.position, p3.position);
    }
}
