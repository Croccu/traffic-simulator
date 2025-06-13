using UnityEngine;

public class TrafficLightSwitcher : MonoBehaviour
{
    public GameObject trafficLightRed;
    public GameObject trafficLightGreen;

    public float redDuration = 5f;
    public float greenDuration = 5f;

    private bool isRed = true;
    private float timer;

    void Start()
    {
        SetState(true); // start with red
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SetState(!isRed); // switch
        }
    }

    void SetState(bool red)
    {
        isRed = red;
        trafficLightRed.SetActive(red);
        trafficLightGreen.SetActive(!red);
        timer = red ? redDuration : greenDuration;
    }
}



