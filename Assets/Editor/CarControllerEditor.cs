using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CarController_v3))]
public class CarController_v3_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("🔁 Reset All CarController Settings"))
        {
            CarController_v3 car = (CarController_v3)target;

            // Reset core settings
            car.maxSpeed = 3f;
            car.rotationSpeed = 200f;
            car.waypointReachThreshold = 0.1f;
            car.bezierStep = 0.02f;

            // Detection
            car.detectionLength = 1f;
            car.detectionWidth = 0.45f;
            car.stopDistance = 0.7f;

            // Lane Switching
            car.parallelDetectRange = 2f;
            car.parallelAngleThreshold = 20f;
            car.switchCooldown = 1.5f;
            car.debugDrawSwitch = false;

            // Lane Tuning
            car.switchDuration = 0.5f;
            car.laneMergeCheckRadius = 0.7f;

            // Rear Detection
            car.rearDetectLength = 0.7f;
            car.rearDetectWidth = 0.45f;

            EditorUtility.SetDirty(car);
        }
        DrawDefaultInspector();
    }
}
