using UnityEngine;

public class WaypointGraph : MonoBehaviour
{
  public static WaypointGraph Instance { get; private set; }
  public WaypointNode[] allNodes;

  void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);

    allNodes = FindObjectsByType<WaypointNode>(FindObjectsSortMode.None);
  }

  public WaypointNode GetRandomEntry()
  {
    var entries = System.Array.FindAll(allNodes, n => n.isEntry);
    return entries.Length > 0 ? entries[Random.Range(0, entries.Length)] : null;
  }

  public WaypointNode GetRandomExit()
  {
    var exits = System.Array.FindAll(allNodes, n => n.isExit);
    return exits.Length > 0 ? exits[Random.Range(0, exits.Length)] : null;
  }
}
