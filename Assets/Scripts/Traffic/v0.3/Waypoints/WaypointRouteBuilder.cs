using UnityEngine;
using System.Collections.Generic;

public class WaypointRouteBuilder : MonoBehaviour
{
  public WaypointRoute route;

  [Tooltip("Spacing between auto-generated waypoints on splines.")]
  [SerializeField] public float splineSpacing = 1f;

  public void BuildRoute()
  {
    if (route == null)
    {
      Debug.LogError("WaypointRoute not assigned.");
      return;
    }

    List<WaypointNode> collected = new();

    foreach (Transform child in transform)
    {
      // CASE 1: Manual waypoints
      WaypointRoute subRoute = child.GetComponent<WaypointRoute>();
      if (subRoute != null)
      {
        collected.AddRange(subRoute.nodes);
        continue;
      }

      // CASE 2: Spline with evenly spaced points
      PathCreator spline = child.GetComponent<PathCreator>();
      if (spline != null && spline.path != null)
      {
        Vector2[] points = spline.path.CalculateEvenlySpacedPoints(splineSpacing);
        foreach (Vector2 p in points)
        {
          GameObject wp = new GameObject("SplineWaypoint");
          wp.transform.position = p;
          wp.transform.SetParent(route.transform);
          WaypointNode node = wp.AddComponent<WaypointNode>();
          collected.Add(node);
        }
      }
    }

    route.nodes = collected;
    Debug.Log("Route built with " + collected.Count + " waypoints.");
  }
}
