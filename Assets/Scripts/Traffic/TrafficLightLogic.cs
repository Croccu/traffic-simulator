using UnityEngine;
using System.Collections;

public class TrafficLightLogic : MonoBehaviour
{
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;

    [Header("Light Timings (seconds)")]
    public float redDuration = 5f;
    public float yellowDuration = 2f;
    public float greenDuration = 5f;

    public enum LightState { Red, Yellow, Green }
    public LightState CurrentState { get; private set; }

    void Start()
    {
        StartCoroutine(SwitchLights());
    }

    IEnumerator SwitchLights()
    {
        while (true)
        {
            SetActiveLight(redLight);
            CurrentState = LightState.Red;
            yield return new WaitForSeconds(5f);

            SetActiveLight(yellowLight);
            CurrentState = LightState.Yellow;
            yield return new WaitForSeconds(2f);

            SetActiveLight(greenLight);
            CurrentState = LightState.Green;
            yield return new WaitForSeconds(5f);
        }
    }

    void SetActiveLight(GameObject active)
    {
        redLight.SetActive(false);
        yellowLight.SetActive(false);
        greenLight.SetActive(false);

        active.SetActive(true);
    }
}