using UnityEngine;
using System.Collections.Generic;

public class TrafficSpawner : MonoBehaviour
{
  public GameObject carPrefab;
  public List<WaypointRoute> availableRoutes;
  public float spawnDelay = 2f;
  public float spawnInterval = 4f;

  void Start()
  {
    InvokeRepeating(nameof(SpawnCar), spawnDelay, spawnInterval);
  }

  void SpawnCar()
  {
    if (availableRoutes.Count == 0 || carPrefab == null) return;

    WaypointRoute route = availableRoutes[Random.Range(0, availableRoutes.Count)];
    if (route.nodes.Count < 2) return;

    Vector3 start = route.nodes[0].transform.position;
    Quaternion rot = Quaternion.LookRotation(Vector3.forward,
                      route.nodes[1].transform.position - start);
    GameObject car = Instantiate(carPrefab, start, rot);

    SplineCarController controller = car.GetComponent<SplineCarController>();;
    if (controller != null)
    {
      controller.SetPath(route.nodes);
    }
  }
}
