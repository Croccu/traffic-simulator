using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficLightController : MonoBehaviour
{
    public List<TrafficLightLogic> pairedLights;

    [Header("Light Timings (seconds)")]
    public float redDuration = 5f;
    public float greenDuration = 5f;

    [Header("One-time Start Delay")]
    public float startDelay = 0f;  // Set in Inspector to offset this group

    void Start()
    {
        StartCoroutine(ControlLights());
    }

    IEnumerator ControlLights()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        while (true)
        {
            // RED
            foreach (var light in pairedLights)
                light.SetActiveLight(TrafficLightLogic.LightState.Red);

            yield return new WaitForSeconds(redDuration);

            // GREEN
            foreach (var light in pairedLights)
                light.SetActiveLight(TrafficLightLogic.LightState.Green);

            yield return new WaitForSeconds(greenDuration);
        }
    }
}
