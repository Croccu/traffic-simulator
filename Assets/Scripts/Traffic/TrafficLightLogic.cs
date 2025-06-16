using UnityEngine;
using System.Collections;

public class TrafficLightLogic : MonoBehaviour
{
    public GameObject redLight;
    public GameObject greenLight;

    [Header("Light Timings (seconds)")]
    public float redDuration = 5f;
    public float greenDuration = 5f;

    [Header("Delay Settings")]
    public float delay = -1f; // If negative, will use redDuration as default

    public enum LightState { Red, Green }
    public LightState CurrentState { get; private set; }

    void Start()
    {
        if (delay < 0f)
            delay = redDuration;

        StartCoroutine(SwitchLights());
    }

    IEnumerator SwitchLights()
    {
        // Delay only once at the start
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        while (true)
        {
            SetActiveLight(LightState.Red);
            yield return new WaitForSeconds(redDuration);

            SetActiveLight(LightState.Green);
            yield return new WaitForSeconds(greenDuration);
        }
    }

    void SetActiveLight(LightState state)
    {
        if (state == LightState.Red)
        {
            redLight.SetActive(true);
            greenLight.SetActive(false);
            CurrentState = LightState.Red;
        }
        else // Green
        {
            redLight.SetActive(false);
            greenLight.SetActive(true);
            CurrentState = LightState.Green;
        }
    }
}