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
    public GameObject carPrefab;
    public float spawnInterval = 3f;
    public float spawnDelay = 2f;

    [Header("Available Routes")]
    public List<Route> availableRoutes = new List<Route>();

    private void Start()
    {
        InvokeRepeating(nameof(SpawnCar), spawnDelay, spawnInterval);
    }

    void SpawnCar()
    {
        if (carPrefab == null || availableRoutes.Count == 0)
        {
            Debug.LogWarning("Spawner setup incomplete.");
            return;
        }

        Route chosenRoute = availableRoutes[Random.Range(0, availableRoutes.Count)];

        if (chosenRoute.spline == null || chosenRoute.spline.waypoints.Count < 2)
        {
            Debug.LogWarning("Invalid spline or insufficient waypoints.");
            return;
        }

        Vector3 startPos = chosenRoute.spline.waypoints[0];
        Vector3 nextPos = chosenRoute.spline.waypoints[1];
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, (nextPos - startPos).normalized);

        GameObject car = Instantiate(carPrefab, startPos, rotation);

        BezierCarController carController = car.GetComponent<BezierCarController>();
        if (carController != null)
        {
            carController.InitializePath(new List<Vector3>(chosenRoute.spline.waypoints));
        }
        else
        {
            Debug.LogError("Car prefab missing BezierCarController.");
        }
    }

    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;
        CancelInvoke(nameof(SpawnCar));
        InvokeRepeating(nameof(SpawnCar), spawnDelay, spawnInterval);
    }
}
