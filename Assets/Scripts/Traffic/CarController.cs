using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
  public List<Waypoint> path;
  public float speed = 5f;
  public float turnSpeed = 20f;

  int currentIndex = 0;
  bool isWaiting = false;
  float originalSpeed;

  void Start()
  {
    originalSpeed = speed;
  }

  void Update()
  {
    if (path == null || currentIndex >= path.Count || isWaiting) return;

    Vector3 target = path[currentIndex].transform.position;
    Vector3 direction = target - transform.position;

    if (direction.magnitude < 0.1f)
    {
      currentIndex++;
      return;
    }

    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime * 100f);
    transform.position += transform.up * speed * Time.deltaTime;
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Intersection"))
    {
      IntersectionManager manager = other.GetComponent<IntersectionManager>();
      if (manager != null)
      {
        Direction entry = GetEntryDirection();
        Turn intent = GetTurnIntent();

        manager.RegisterCar(gameObject, entry, intent);
        BeginIntersectionWait(manager, entry, intent);
      }
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("Intersection"))
    {
      IntersectionManager manager = other.GetComponent<IntersectionManager>();
      if (manager != null)
      {
        manager.UnregisterCar(gameObject);
      }
    }
  }

  public void BeginIntersectionWait(IntersectionManager manager, Direction entry, Turn intent)
  {
    StartCoroutine(WaitForPriority(manager, entry, intent));
  }

  IEnumerator WaitForPriority(IntersectionManager manager, Direction entry, Turn intent)
  {
    isWaiting = true;
    speed = 0;

    while (!manager.HasPriority(gameObject, entry, intent))
    {
      yield return null;
    }

    speed = originalSpeed;
    isWaiting = false;
  }

  public Direction GetEntryDirection()
  {
    Vector3 dir = transform.up;

    if (Vector3.Dot(dir, Vector3.up) > 0.7f) return Direction.North;
    if (Vector3.Dot(dir, Vector3.down) > 0.7f) return Direction.South;
    if (Vector3.Dot(dir, Vector3.left) > 0.7f) return Direction.West;
    return Direction.East;
  }

  public Turn GetTurnIntent()
  {
    if (path == null || currentIndex + 1 >= path.Count) return Turn.Straight;

    Vector3 currentDir = path[currentIndex].transform.position - transform.position;
    Vector3 nextDir = path[currentIndex + 1].transform.position - path[currentIndex].transform.position;

    float angle = Vector3.SignedAngle(currentDir.normalized, nextDir.normalized, Vector3.forward);

    if (angle > 30f) return Turn.Left;
    if (angle < -30f) return Turn.Right;
    return Turn.Straight;
  }
}
