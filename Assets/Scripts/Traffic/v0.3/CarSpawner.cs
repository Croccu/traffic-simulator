using UnityEngine;

public class CarSpawner : MonoBehaviour
{
  public GameObject carPrefab;
  public float spawnInterval = 3f;

  void Start()
  {
    InvokeRepeating(nameof(SpawnCar), 2f, spawnInterval);
  }

  public void SetSpawnInterval(float interval)
  {
    spawnInterval = interval;
  }

  void SpawnCar()
  {
    if (carPrefab == null) return;

    WaypointNode entry = WaypointGraph.Instance.GetRandomEntry();
    if (entry == null) return;

    Vector3 spawnPos = entry.transform.position;
    Quaternion spawnRot = Quaternion.identity;

    if (entry.connectedNodes.Count > 0)
    {
      Vector3 dir = entry.connectedNodes[0].transform.position - entry.transform.position;
      spawnRot = Quaternion.LookRotation(Vector3.forward, dir.normalized);
    }

    GameObject car = Instantiate(carPrefab, spawnPos, spawnRot);
  }
}
