using UnityEngine;

public class StopZone : MonoBehaviour
{
    public Collider2D detectionArea;

    public bool IsIntersectionClear(Collider2D self)
    {
        Collider2D[] others = Physics2D.OverlapBoxAll(detectionArea.bounds.center, detectionArea.bounds.size, 0f);
        foreach (var col in others)
        {
            if (col != self && col.CompareTag("Car"))
                return false;
        }
        return true;
    }
}
