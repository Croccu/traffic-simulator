using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public enum LightState { Red, Yellow, Green }
    public LightState currentState = LightState.Red;

    public float redDuration = 5f;
    public float yellowDuration = 2f;
    public float greenDuration = 10f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case LightState.Red:
                if (timer >= redDuration)
                {
                    currentState = LightState.Green;
                    timer = 0f;
                }
                break;

            case LightState.Yellow:
                if (timer >= yellowDuration)
                {
                    currentState = LightState.Red;
                    timer = 0f;
                }
                break;

            case LightState.Green:
                if (timer >= greenDuration)
                {
                    currentState = LightState.Yellow;
                    timer = 0f;
                }
                break;
        }
    }

    public bool IsRed()
    {
        return currentState == LightState.Red;
    }
}