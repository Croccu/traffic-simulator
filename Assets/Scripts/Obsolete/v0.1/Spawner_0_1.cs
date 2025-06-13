using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
  public GameObject carPrefab;
  public Waypoint spawnWaypoint;
  public float spawnInterval = 3f;

  private Waypoint[] exitWaypoints;

  void Start()
  {
    GameObject[] exitObjects = GameObject.FindGameObjectsWithTag("Exit");
    exitWaypoints = new Waypoint[exitObjects.Length];
    for (int i = 0; i < exitObjects.Length; i++)
      exitWaypoints[i] = exitObjects[i].GetComponent<Waypoint>();
  }

  public void StartSpawning()
  {
    InvokeRepeating(nameof(SpawnCar), 2f, spawnInterval);
  }

  public void SetSpawnInterval(float interval)
  {
    spawnInterval = interval;
  }

  void SpawnCar()
  {
    if (carPrefab == null || spawnWaypoint == null || exitWaypoints.Length == 0)
      return;

    Waypoint chosenExit = GetRandomExit(spawnWaypoint);
    if (chosenExit == null) return;

    var path = Pathfinding.FindPath(spawnWaypoint, chosenExit);
    if (path == null || path.Count < 2) return;

    Vector3 nextDir = path[1].transform.position - path[0].transform.position;
    Quaternion instantRotation = Quaternion.LookRotation(Vector3.forward, nextDir.normalized);

    GameObject car = Instantiate(carPrefab);
    car.transform.rotation = instantRotation;

    Vector3 offset = -car.transform.up * 0.5f;
    car.transform.position = spawnWaypoint.transform.position + offset;

    CarController carScript = car.GetComponent<CarController>();
    carScript.InitializePath(path);
  }

  Waypoint GetRandomExit(Waypoint from)
  {
    List<Waypoint> candidates = new List<Waypoint>();
    foreach (var exit in exitWaypoints)
    {
      if (exit == null) continue;
      if (exit.name.Contains(from.name.Split('_')[1])) continue;
      candidates.Add(exit);
    }
    return candidates.Count == 0 ? null : candidates[Random.Range(0, candidates.Count)];
  }
}
