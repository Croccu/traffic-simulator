using UnityEngine;

public class WaypointNode : MonoBehaviour
{
  [Tooltip("Should the car stop here?")]
  public bool isStopPoint = false;

  [Tooltip("Custom rotation target (optional).")]
  public Transform overrideLookTarget;
}
