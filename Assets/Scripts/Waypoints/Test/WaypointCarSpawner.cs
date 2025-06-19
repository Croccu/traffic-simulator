using UnityEngine;
using System.Collections.Generic;

public class WaypointCarSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public List<GameObject> carPrefabs;
    public float spawnInterval = 3f;
    public float spawnDelay = 1f;
    public int maxCarsToSpawn = 10; // UUS: max kogus

    private bool isSpawning = false;
    private int carsSpawned = 0; // UUS: loendur

    void Start()
    {
        // Optional auto-start
        // StartSpawning();
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            carsSpawned = 0; // Alustame nullist

            if (GameManager.instance != null)
            {
                GameManager.instance?.AddToSpawnTarget(maxCarsToSpawn);
            }

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
        if (carsSpawned >= maxCarsToSpawn)
        {
            StopSpawning();
            return;
        }

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

        carsSpawned++; // UUS: suurenda loendurit
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