using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BezierRouteSpline : MonoBehaviour
{
	public List<Transform> controlPoints = new List<Transform>();

	[Range(2, 100)]
	public int resolutionPerSegment = 20;

	[HideInInspector]
	public List<Vector3> waypoints = new List<Vector3>();

	public List<BezierRouteSpline> nextSplines = new List<BezierRouteSpline>();

	public void GenerateWaypoints()
	{
		waypoints.Clear();

		if (controlPoints.Count == 2)
		{
			for (int i = 0; i <= resolutionPerSegment; i++)
			{
				float t = i / (float)resolutionPerSegment;
				Vector3 point = Vector3.Lerp(controlPoints[0].position, controlPoints[1].position, t);
				waypoints.Add(point);
			}
			return;
		}

		if (controlPoints.Count < 4 || controlPoints.Count % 3 != 1)
		{
			Debug.LogWarning($"{name}: Control points must follow the 4 + 3n pattern.");
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

	private Vector3 CalculateBezier(float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		return Mathf.Pow(1 - t, 3) * a +
		       3 * Mathf.Pow(1 - t, 2) * t * b +
		       3 * (1 - t) * Mathf.Pow(t, 2) * c +
		       Mathf.Pow(t, 3) * d;
	}

	public Vector3 GetPointAt(float t)
	{
		if (controlPoints.Count < 4 || controlPoints.Count % 3 != 1)
			return transform.position;

		int segmentCount = (controlPoints.Count - 1) / 3;
		t = Mathf.Clamp01(t);
		float scaledT = t * segmentCount;
		int segment = Mathf.Min(Mathf.FloorToInt(scaledT), segmentCount - 1);
		float segmentT = scaledT - segment;

		Vector3 a = controlPoints[segment * 3].position;
		Vector3 b = controlPoints[segment * 3 + 1].position;
		Vector3 c = controlPoints[segment * 3 + 2].position;
		Vector3 d = controlPoints[segment * 3 + 3].position;

		return CalculateBezier(segmentT, a, b, c, d);
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

		if (waypoints.Count > 0)
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
			Gizmos.DrawSphere(waypoints[0], 0.15f);

			Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
			Gizmos.DrawSphere(waypoints[waypoints.Count - 1], 0.15f);
		}

		if (waypoints.Count > 0)
		{
			foreach (var next in nextSplines)
			{
				if (next != null && next.waypoints.Count > 0)
				{
					Vector3 endA = waypoints[waypoints.Count - 1];
					Vector3 startB = next.waypoints[0];
					float distance = Vector3.Distance(endA, startB);

					Gizmos.color = distance < 0.5f ? Color.green : Color.red;
					Gizmos.DrawLine(endA, startB);
				}
			}
		}
	}

	public void ConnectTo(BezierRouteSpline other)
	{
		if (other != null && !nextSplines.Contains(other))
			nextSplines.Add(other);
	}
}
