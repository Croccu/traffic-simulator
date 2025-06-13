using UnityEngine;

public class GiveWayZone : MonoBehaviour
{
    public float detectionRadius = 2f;
    public Vector2 detectionOffset = Vector2.zero;  // Local space offset (e.g. (0, 1))
    public LayerMask carLayer;

    public bool IsIntersectionClear(Collider2D requester)
    {
        Vector2 worldCenter = (Vector2)transform.position + (Vector2)transform.TransformDirection(detectionOffset);

        Collider2D[] cars = Physics2D.OverlapCircleAll(worldCenter, detectionRadius, carLayer);

        foreach (Collider2D car in cars)
        {
            if (car != requester)
            {
                return false;
            }
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + transform.TransformDirection(detectionOffset);
        Gizmos.DrawWireSphere(center, detectionRadius);
    }
}
