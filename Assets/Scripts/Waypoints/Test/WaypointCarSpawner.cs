using UnityEngine;
using System.Collections.Generic;

public class WaypointCarSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public List<GameObject> carPrefabs;
    public float spawnInterval = 3f;
    public float spawnDelay = 1f;

    private bool isSpawning = false;

    void Start()
    {
        // Optional auto-start
        // StartSpawning();
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            InvokeRepeating(nameof(SpawnCar), spawnDelay, spawnInterval);
            isSpawning = true;
        }
    }

    public void StopSpawning()
    {
        if (isSpawning)
        {
            CancelInvoke(nameof(SpawnCar));
            isSpawning = false;
        }
    }

    public void ToggleSpawning()
    {
        if (isSpawning)
            StopSpawning();
        else
            StartSpawning();
    }

    void SpawnCar()
    {
        if (carPrefabs.Count == 0)
        {
            Debug.LogWarning("No car prefabs assigned.");
            return;
        }

        WaypointNode[] allWaypoints = Object.FindObjectsByType<WaypointNode>(FindObjectsSortMode.None);
        List<WaypointNode> entries = new();

        foreach (var node in allWaypoints)
        {
            if (node.isEntry)
                entries.Add(node);
        }

        if (entries.Count == 0)
        {
            Debug.LogWarning("No entry waypoints found.");
            return;
        }

        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
        WaypointNode entry = entries[Random.Range(0, entries.Count)];

        Quaternion rotation = Quaternion.identity;
        if (entry.nextNodes.Count > 0)
        {
            Vector2 dir = (entry.nextNodes[0].transform.position - entry.transform.position).normalized;
            rotation = Quaternion.LookRotation(Vector3.forward, dir);
        }

        GameObject car = Instantiate(prefab, entry.transform.position, rotation);
        CarController_v3 controller = car.GetComponent<CarController_v3>();
        if (controller != null)
            controller.SetStartNode(entry);
    }

    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;
        if (isSpawning)
        {
            CancelInvoke(nameof(SpawnCar));
            InvokeRepeating(nameof(SpawnCar), spawnDelay, spawnInterval);
        }
    }
}
