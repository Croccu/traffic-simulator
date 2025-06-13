using System.Collections.Generic;
using UnityEngine;

public enum Direction { North, East, South, West }
public enum Turn { Left, Right, Straight }

public class CarIntent
{
  public GameObject car;
  public Direction entryDirection;
  public Turn turn;
  public bool isReady = true;
}

public class IntersectionManager : MonoBehaviour
{
  private List<CarIntent> carsInIntersection = new List<CarIntent>();
  private GameObject leftTurnActiveCar = null;

  public void RegisterCar(GameObject car, Direction from, Turn turn)
  {
    if (carsInIntersection.Exists(c => c.car == car)) return;

    carsInIntersection.Add(new CarIntent
    {
      car = car,
      entryDirection = from,
      turn = turn,
      isReady = true
    });
  }

  public void UnregisterCar(GameObject car)
  {
    carsInIntersection.RemoveAll(c => c.car == car);

    if (leftTurnActiveCar == car)
      leftTurnActiveCar = null;
  }

  public void SetCarReady(GameObject car, bool ready)
  {
    var intent = carsInIntersection.Find(c => c.car == car);
    if (intent != null)
      intent.isReady = ready;
  }

  public bool HasPriority(GameObject car, Direction from, Turn turn)
  {
    foreach (var other in carsInIntersection)
    {
      if (other.car == car || !other.isReady) continue;

      // Rule 1: Car on the right has priority
      if (IsToTheRightOf(from, other.entryDirection))
        return false;

      // Rule 2: Left-turn yields to oncoming traffic
      if (turn == Turn.Left)
      {
        bool isOpposite = IsOpposite(from, other.entryDirection);
        if (isOpposite && (other.turn == Turn.Straight || other.turn == Turn.Right))
          return false;

        if (leftTurnActiveCar != null && leftTurnActiveCar != car)
          return false;
      }
    }

    return true;
  }

  public bool ClaimLeftTurnSlot(GameObject car)
  {
    if (leftTurnActiveCar == null || leftTurnActiveCar == car)
    {
      leftTurnActiveCar = car;
      return true;
    }

    return false;
  }

  private bool IsToTheRightOf(Direction me, Direction other)
  {
    return
      (me == Direction.North && other == Direction.East) ||
      (me == Direction.East && other == Direction.South) ||
      (me == Direction.South && other == Direction.West) ||
      (me == Direction.West && other == Direction.North);
  }

  private bool IsOpposite(Direction a, Direction b)
  {
    return
      (a == Direction.North && b == Direction.South) ||
      (a == Direction.South && b == Direction.North) ||
      (a == Direction.East && b == Direction.West) ||
      (a == Direction.West && b == Direction.East);
  }
}
