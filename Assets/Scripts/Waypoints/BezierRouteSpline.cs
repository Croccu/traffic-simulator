using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BezierRouteSpline : MonoBehaviour
{
    [Header("Control Points (4 + 3n)")]
    public List<Transform> controlPoints = new List<Transform>();

    [Range(2, 100)]
    public int resolutionPerSegment = 20;

    [HideInInspector]
    public List<Vector3> waypoints = new List<Vector3>();

    [Header("Connected Next Splines")]
    public List<BezierRouteSpline> nextSplines = new List<BezierRouteSpline>();

    public void GenerateWaypoints()
    {
        waypoints.Clear();

        if (controlPoints.Count < 4 || controlPoints.Count % 3 != 1)
        {
            Debug.LogWarning($"{name}: Control points must follow the 4 + 3n pattern.");
            return;
        }

        for (int i = 0; i < (controlPoints.Count - 1) / 3; i++)
        {
            Transform t0 = controlPoints[i * 3];
            Transform t1 = controlPoints[i * 3 + 1];
            Transform t2 = controlPoints[i * 3 + 2];
            Transform t3 = controlPoints[i * 3 + 3];

            if (t0 == null || t1 == null || t2 == null || t3 == null)
                continue;

            for (int j = 0; j <= resolutionPerSegment; j++)
            {
                float t = j / (float)resolutionPerSegment;
                Vector3 point = CalculateBezier(t, t0.position, t1.position, t2.position, t3.position);
                waypoints.Add(point);
            }
        }
    }

    private Vector3 CalculateBezier(float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        return Mathf.Pow(1 - t, 3) * a +
               3 * Mathf.Pow(1 - t, 2) * t * b +
               3 * (1 - t) * Mathf.Pow(t, 2) * c +
               Mathf.Pow(t, 3) * d;
    }

    private void Start()
    {
        if (waypoints == null || waypoints.Count < 2)
            GenerateWaypoints();
    }

    private void OnDrawGizmos()
    {
        GenerateWaypoints();

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }

        Gizmos.color = Color.gray;
        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            if (controlPoints[i] != null && controlPoints[i + 1] != null)
                Gizmos.DrawLine(controlPoints[i].position, controlPoints[i + 1].position);
        }

        Gizmos.color = Color.cyan;
        if (waypoints.Count > 0)
        {
            foreach (var next in nextSplines)
            {
                if (next != null && next.waypoints.Count > 0)
                {
                    Gizmos.DrawLine(waypoints[waypoints.Count - 1], next.waypoints[0]);
                }
            }
        }
    }

    public void ConnectTo(BezierRouteSpline other)
    {
        if (other != null && !nextSplines.Contains(other))
        {
            nextSplines.Add(other);
        }
    }
}
