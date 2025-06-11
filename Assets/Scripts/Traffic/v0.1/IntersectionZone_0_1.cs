using UnityEngine;
using System.Collections.Generic;

public class IntersectionZone : MonoBehaviour
{
  private HashSet<CarController> carsInside = new HashSet<CarController>();

  public bool IsClear()
  {
    return carsInside.Count == 0;
  }

  public void Register(CarController car)
  {
    carsInside.Add(car);
  }

  public void Unregister(CarController car)
  {
    carsInside.Remove(car);
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    CarController car = other.GetComponent<CarController>();
    if (car != null)
    {
      Register(car);
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    CarController car = other.GetComponent<CarController>();
    if (car != null)
    {
      Unregister(car);
    }
  }
}
