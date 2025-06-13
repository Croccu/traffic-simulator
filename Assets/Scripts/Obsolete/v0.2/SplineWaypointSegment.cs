using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PathCreator))]
public class SplineWaypointSegment : MonoBehaviour
{
  public float spacing = 0.4f;
  public GameObject waypointPrefab;

  [ContextMenu("Generate Waypoints")]
  public void GenerateWaypoints()
  {
    PathCreator creator = GetComponent<PathCreator>();
    if (creator == null || creator.path == null)
    {
      Debug.LogError("Missing PathCreator or path.");
      return;
    }

    Vector2[] points = creator.path.CalculateEvenlySpacedPoints(spacing);
    List<WaypointNode> nodes = new();

    foreach (Transform child in transform)
    {
      if (Application.isEditor) DestroyImmediate(child.gameObject);
      else Destroy(child.gameObject);
    }

    for (int i = 0; i < points.Length; i++)
    {
      GameObject wp = Instantiate(waypointPrefab, points[i], Quaternion.identity, transform);
      wp.name = $"{gameObject.name}_Waypoint_{i}";

      WaypointNode node = wp.GetComponent<WaypointNode>();
      if (node == null) node = wp.AddComponent<WaypointNode>();

      nodes.Add(node);
    }

    for (int i = 0; i < nodes.Count - 1; i++)
    {
      nodes[i].connectedNodes.Add(nodes[i + 1]);
    }
  }
}
