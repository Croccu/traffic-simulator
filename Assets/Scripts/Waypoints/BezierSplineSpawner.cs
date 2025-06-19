using UnityEngine;
using System.Collections.Generic;

public class BezierSplineSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Route
    {
        public string name;
        public BezierRouteSpline spline;
    }

    [Header("Spawner Settings")]
    public List<GameObject> carPrefabs;
    public float spawnInterval = 3f;
    public float spawnDelay = 2f;
    public int maxCarsToSpawn = 10; // NEW: max amount to spawn

    [Header("Available Routes")]
    public List<Route> availableRoutes = new List<Route>();

    private bool isSpawning = false;
    private int carsSpawned = 0; // NEW: tracking count

    private void Start()
    {
        // Optional auto-start:
        // StartSpawning();
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            carsSpawned = 0;

            // NEW: Start level timer in GameManager
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

        if (carPrefabs.Count == 0 || availableRoutes.Count == 0)
        {
            Debug.LogWarning("Spawner setup incomplete.");
            return;
        }

        GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
        Route chosenRoute = availableRoutes[Random.Range(0, availableRoutes.Count)];

        if (carPrefab == null || chosenRoute.spline == null || chosenRoute.spline.waypoints.Count < 2)
        {
            Debug.LogWarning("Invalid car prefab or spline.");
            return;
        }

        Vector3 startPos = chosenRoute.spline.waypoints[0];
        Vector3 nextPos = chosenRoute.spline.waypoints[1];
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, (nextPos - startPos).normalized);

        GameObject car = Instantiate(carPrefab, startPos, rotation);

        BezierCarController carController = car.GetComponent<BezierCarController>();
        if (carController != null)
        {
            carController.SetCurrentSpline(chosenRoute.spline);
            carController.InitializePath(new List<Vector3>(chosenRoute.spline.waypoints));
        }
        else
        {
            Debug.LogError("Car prefab is missing BezierCarController.");
        }

        carsSpawned++; // NEW: increment count
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