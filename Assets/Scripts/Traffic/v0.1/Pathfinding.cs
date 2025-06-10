using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Pathfinding
{
  public static List<Waypoint> FindPath(Waypoint start, Waypoint goal)
  {
    var openSet = new List<Waypoint> { start };
    var cameFrom = new Dictionary<Waypoint, Waypoint>();

    var gScore = new Dictionary<Waypoint, float> { [start] = 0 };
    var fScore = new Dictionary<Waypoint, float> { [start] = Heuristic(start, goal) };

    while (openSet.Count > 0)
    {
      // Get node in openSet with lowest fScore
      Waypoint current = openSet.OrderBy(n => fScore.ContainsKey(n) ? fScore[n] : Mathf.Infinity).First();

      if (current == goal)
        return ReconstructPath(cameFrom, current);
      openSet.Remove(current);

      foreach (Waypoint neighbor in current.connectedWaypoints)
      {
        float tentativeG = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

        if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
        {
          cameFrom[neighbor] = current;
          gScore[neighbor] = tentativeG;
          fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

          if (!openSet.Contains(neighbor))
          openSet.Add(neighbor);
        }
      }
    }

    Debug.LogWarning("No path found from " + start.name + " to " + goal.name);
    return null;
  }

  private static float Heuristic(Waypoint a, Waypoint b)
  {
      return Vector3.Distance(a.transform.position, b.transform.position);
  }

  private static List<Waypoint> ReconstructPath(Dictionary<Waypoint, Waypoint> cameFrom, Waypoint current)
  {
      var totalPath = new List<Waypoint> { current };
      while (cameFrom.ContainsKey(current))
      {
          current = cameFrom[current];
          totalPath.Insert(0, current);
      }
      return totalPath;
  }
}
