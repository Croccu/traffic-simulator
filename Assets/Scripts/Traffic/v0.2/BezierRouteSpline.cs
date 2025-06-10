using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BezierRouteSpline : MonoBehaviour
{
    [Header("Control Points (in multiples of 4)")]
    public List<Transform> controlPoints = new List<Transform>();

    [Range(2, 100)]
    public int resolutionPerSegment = 20;

    [HideInInspector]
    public List<Vector3> waypoints = new List<Vector3>();

    public void GenerateWaypoints()
    {
        waypoints.Clear();

        if (controlPoints.Count < 4 || controlPoints.Count % 3 != 1)
        {
            Debug.LogWarning("Control points must be 4 + (n * 3). E.g. 4, 7, 10, 13...");
            return;
        }

        for (int i = 0; i < (controlPoints.Count - 1) / 3; i++)
        {
            Vector3 p0 = controlPoints[i * 3].position;
            Vector3 p1 = controlPoints[i * 3 + 1].position;
            Vector3 p2 = controlPoints[i * 3 + 2].position;
            Vector3 p3 = controlPoints[i * 3 + 3].position;

            for (int j = 0; j <= resolutionPerSegment; j++)
            {
                float t = j / (float)resolutionPerSegment;
                Vector3 point = CalculateBezier(t, p0, p1, p2, p3);
                waypoints.Add(point);
            }
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
        if (controlPoints.Count < 4 || controlPoints.Count % 3 != 1)
            return;

        GenerateWaypoints();

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }

        Gizmos.color = Color.gray;
        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(controlPoints[i].position, controlPoints[i + 1].position);
        }
    }
}
