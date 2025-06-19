using UnityEngine;

[RequireComponent(typeof(CarController_v3))]
public class CarDebugGizmos : MonoBehaviour
{
    private CarController_v3 car;

    void Awake()
    {
        car = GetComponent<CarController_v3>();
    }

    void OnDrawGizmos()
    {
        if (car == null)
            car = GetComponent<CarController_v3>();

        // Front detection capsule
        Vector2 center = (Vector2)transform.position + (Vector2)transform.up * (car.detectionLength * 0.5f);
        Vector2 size = new Vector2(car.detectionWidth, car.detectionLength);
        float angle = transform.eulerAngles.z;

        Gizmos.color = new Color(1f, 0.2f, 0f, 0.3f); // orange-red
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;

        // Rear detection box
        Vector2 backCenter = (Vector2)transform.position - (Vector2)transform.up * (car.rearDetectLength * 0.5f);
        Vector2 rearSize = new Vector2(car.rearDetectWidth, car.rearDetectLength);
        Matrix4x4 rearMatrix = Matrix4x4.TRS(backCenter, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.color = new Color(0f, 0.3f, 1f, 0.2f); // blue
        Gizmos.matrix = rearMatrix;
        Gizmos.DrawCube(Vector3.zero, rearSize);
        Gizmos.matrix = Matrix4x4.identity;
    }
}

