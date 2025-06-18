using UnityEngine;

public class TrafficLightLogic : MonoBehaviour
{
    public GameObject redLight;
    public GameObject greenLight;

    public enum LightState { Red, Green }
    public LightState CurrentState { get; private set; }

    public void SetActiveLight(LightState state)
    {
        // Prevent switching if not playing
        if (!UiButtonController.IsPlaying)
            return;

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
