using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
  public GameObject carPrefab;
  public Waypoint spawnWaypoint;
  public float spawnInterval = 3f;
  public bool randomSpawn = false; // Toggle for random spawn intervals

  private Waypoint[] exitWaypoints;

  void Start()
  {
    // Cache all exit waypoints (tagged "Exit")
    GameObject[] exitObjects = GameObject.FindGameObjectsWithTag("Exit");
    exitWaypoints = new Waypoint[exitObjects.Length];

    for (int i = 0; i < exitObjects.Length; i++)
    {
      exitWaypoints[i] = exitObjects[i].GetComponent<Waypoint>();
      if (exitWaypoints[i] == null)
      {
        Debug.LogWarning($"GameObject '{exitObjects[i].name}' tagged as 'Exit' is missing a Waypoint component.");
      }
    }

    if (exitWaypoints.Length == 0)
    {
      Debug.LogWarning("No GameObjects tagged as 'Exit' found in the scene.");
    }

    StartSpawning();
  }

  void StartSpawning()
  {
    if (randomSpawn)
    {
      StartCoroutine(RandomSpawnCoroutine());
    }
    else
    {
      InvokeRepeating(nameof(SpawnCar), 2f, spawnInterval);
    }
  }

  IEnumerator RandomSpawnCoroutine()
  {
    while (true)
    {
      SpawnCar();
      float randomInterval = Random.Range(1.5f, 5f); // Random interval between 1.5 and 5 seconds
      yield return new WaitForSeconds(randomInterval);
    }
  }

  void SpawnCar()
  {
    if (carPrefab == null || spawnWaypoint == null || exitWaypoints.Length == 0)
    {
      Debug.LogWarning("Spawner is missing prefab or exit points!");
      return;
    }

    GameObject car = Instantiate(carPrefab, spawnWaypoint.transform.position, Quaternion.identity);
    CarController carScript = car.GetComponent<CarController>();

    // Choose the closest exit
    Waypoint closestExit = GetClosestExit(spawnWaypoint);

    // Generate the path using A* algorithm
    var path = Pathfinding.FindPath(spawnWaypoint, closestExit);
    carScript.path = path;
  }

  Waypoint GetClosestExit(Waypoint from)
  {
    Waypoint closest = null;
    float minDist = Mathf.Infinity;

    foreach (var exit in exitWaypoints)
    {
      if (exit == null) continue;

      float dist = Vector3.Distance(from.transform.position, exit.transform.position);
      if (dist < minDist)
      {
        minDist = dist;
        closest = exit;
      }
    }

    return closest;
  }
}