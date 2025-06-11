using UnityEngine;
using System.Collections.Generic;

public class BezierPathSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject carPrefab;
    public float spawnInterval = 3f;

    [Header("Route Options")]
    public List<BezierRoute> availableRoutes = new List<BezierRoute>();

    private bool isSpawning = false;

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            InvokeRepeating(nameof(SpawnCar), 0f, spawnInterval);
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
        if (availableRoutes.Count == 0 || carPrefab == null)
            return;

        BezierRoute route = availableRoutes[Random.Range(0, availableRoutes.Count)];
        if (route == null || route.waypoints.Count < 2)
        {
            Debug.LogWarning("Invalid route selected.");
            return;
        }

        Vector3 start = route.waypoints[0];
        Vector3 next = route.waypoints[1];
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, (next - start).normalized);

        GameObject car = Instantiate(carPrefab, start, rot);

        BezierCarController controller = car.GetComponent<BezierCarController>();
        if (controller)
            controller.InitializePath(new List<Vector3>(route.waypoints));
    }
}
