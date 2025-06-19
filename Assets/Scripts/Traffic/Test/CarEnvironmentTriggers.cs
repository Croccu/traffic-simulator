using UnityEngine;

public class CarEnvironmentTriggers : MonoBehaviour
{
    private CarController_v3 car;
    private Collider2D selfCollider;

    private GiveWayZone currentGiveWayZone = null;
    private StopZone currentStopZone = null;
    private TrafficLightLogic currentTrafficLight = null;
    private bool isStoppingTemporarily = false;
    private float stopReleaseTime = 0f;

    void Awake()
    {
        car = GetComponent<CarController_v3>();
        selfCollider = GetComponent<Collider2D>();
    }

    public bool IsEnvironmentBlocking()
    {
        // Traffic Light RED
        if (currentTrafficLight != null && currentTrafficLight.CurrentState == TrafficLightLogic.LightState.Red)
            return true;

        // Temporary StopMarker
        if (isStoppingTemporarily)
        {
            if (Time.time < stopReleaseTime)
                return true;
            else
                isStoppingTemporarily = false;
        }

        // GiveWayZone
        if (currentGiveWayZone != null && !currentGiveWayZone.IsIntersectionClear(selfCollider))
            return true;

        // StopZone
        if (currentStopZone != null && !currentStopZone.IsIntersectionClear(selfCollider))
            return true;

        return false;
    }

    public void ClearGiveWayFlagIfPossible()
    {
        if (currentGiveWayZone != null && currentGiveWayZone.IsIntersectionClear(selfCollider))
            currentGiveWayZone = null;

        if (currentStopZone != null && currentStopZone.IsIntersectionClear(selfCollider))
            currentStopZone = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{name} hit trigger: {other.name}");

        if (other.TryGetComponent(out GiveWayZone giveWay))
        {
            currentGiveWayZone = giveWay;
        }

        if (other.TryGetComponent(out StopZone stopZone))
        {
            currentStopZone = stopZone;
        }

        if (other.TryGetComponent(out SpeedZone speedZone))
        {
            car.currentSpeedLimit = speedZone.speedLimit;
        }

        if (other.TryGetComponent(out StopMarker stopMarker))
        {
            isStoppingTemporarily = true;
            stopReleaseTime = Time.time + stopMarker.stopDuration;
        }

        // Check for TrafficLightLogic component directly or in parent
        if (other.TryGetComponent(out TrafficLightLogic trafficLight))
        {
            currentTrafficLight = trafficLight;
        }
        else
        {
            currentTrafficLight = other.GetComponentInParent<TrafficLightLogic>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<GiveWayZone>() == currentGiveWayZone)
            currentGiveWayZone = null;

        if (other.GetComponent<StopZone>() == currentStopZone)
            currentStopZone = null;

        if ((other.GetComponent<TrafficLightLogic>() == currentTrafficLight ||
             other.GetComponentInParent<TrafficLightLogic>() == currentTrafficLight))
        {
            currentTrafficLight = null;
        }
    }
}
