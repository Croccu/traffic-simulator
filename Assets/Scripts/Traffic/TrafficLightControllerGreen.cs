using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficLightControllerGreen : MonoBehaviour
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
            // Wait until playing
            while (!UiButtonController.IsPlaying)
                yield return null;

            // RED
            foreach (var light in pairedLights)
                light.SetActiveLight(TrafficLightLogic.LightState.Red);

            float redTimer = 0f;
            while (redTimer < redDuration)
            {
                if (!UiButtonController.IsPlaying)
                    yield return new WaitUntil(() => UiButtonController.IsPlaying);
                redTimer += Time.deltaTime;
                yield return null;
            }

            // GREEN
            foreach (var light in pairedLights)
                light.SetActiveLight(TrafficLightLogic.LightState.Green);

            float greenTimer = 0f;
            while (greenTimer < greenDuration)
            {
                if (!UiButtonController.IsPlaying)
                    yield return new WaitUntil(() => UiButtonController.IsPlaying);
                greenTimer += Time.deltaTime;
                yield return null;
            }
        }
    }
}