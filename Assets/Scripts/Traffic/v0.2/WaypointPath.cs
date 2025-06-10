using UnityEngine;
using System.Collections.Generic;

public class WaypointPath : MonoBehaviour
{
    public List<Transform> controlPoints = new List<Transform>();
    public List<Vector3> waypoints = new List<Vector3>();
    public int resolution = 20;

    public void GenerateWaypoints()
    {
        waypoints.Clear();
        if (controlPoints.Count < 4) return;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = CalculateCubicBezier(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position);
            waypoints.Add(point);
        }
    }

    Vector3 CalculateCubicBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Mathf.Pow(1 - t, 3) * p0 +
               3 * Mathf.Pow(1 - t, 2) * t * p1 +
               3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
               Mathf.Pow(t, 3) * p3;
    }
}
