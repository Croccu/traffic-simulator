using System.Collections.Generic;
using UnityEngine;

public class CarDetectionLogic : MonoBehaviour
{
    public float GetFrontTargetSpeed(CarController_v3 car)
    {
        Vector2 center = (Vector2)car.transform.position + (Vector2)car.transform.up * (car.detectionLength * 0.65f);
        Vector2 size = new Vector2(car.detectionWidth, car.detectionLength);
        float angle = car.transform.eulerAngles.z;

        Collider2D hit = Physics2D.OverlapCapsule(center, size, CapsuleDirection2D.Vertical, angle, car.carLayer);

        if (hit != null && hit != car.GetComponent<Collider2D>())
        {
            CarController_v3 carAhead = hit.GetComponent<CarController_v3>();
            if (carAhead == car) return car.currentSpeedLimit;

            float dist = Vector2.Distance(car.transform.position, hit.transform.position);
            float spacing = car.stopDistance + 0.7f;

            // Ignore opposite direction cars if far enough
            if (carAhead != null)
            {
                Vector2 myForward = car.transform.up;
                Vector2 theirForward = carAhead.transform.up;
                float dot = Vector2.Dot(myForward.normalized, theirForward.normalized);

                if (dot < 0.25f && dist > spacing + 0.2f)
                    return car.currentSpeedLimit;

                // If the car ahead is currently stopped, block earlier
                if (carAhead.currentSpeed < 0.01f && dist < spacing + 0.3f)
                {
                    Debug.DrawLine(car.transform.position, hit.transform.position, Color.red);
                    return 0f;
                }
            }

            // Force full stop if spacing violated
            if (dist < spacing)
            {
                Debug.DrawLine(car.transform.position, hit.transform.position, Color.red);
                return 0f;
            }

            // Smooth slow if still safe distance
            float t = (dist - spacing) / (car.detectionLength - spacing);
            Debug.DrawLine(car.transform.position, hit.transform.position, Color.yellow);
            return Mathf.Lerp(0f, car.currentSpeedLimit, Mathf.Clamp01(t));
        }

        return car.currentSpeedLimit;
    }



    public bool IsRearCarTooClose(CarController_v3 car)
    {
        Vector2 backCenter = (Vector2)car.transform.position - (Vector2)car.transform.up * (car.rearDetectLength * 0.5f);
        Vector2 rearSize = new Vector2(car.rearDetectWidth, car.rearDetectLength);
        float angle = car.transform.eulerAngles.z;

        Collider2D rearHit = Physics2D.OverlapCapsule(backCenter, rearSize, CapsuleDirection2D.Vertical, angle, car.carLayer);
        if (rearHit != null && rearHit != car.GetComponent<Collider2D>())
        {
            Rigidbody2D rb = rearHit.attachedRigidbody;
            if (rb != null)
            {
                Vector2 toMe = (Vector2)car.transform.position - rb.position;
                Vector2 rearVel = rb.linearVelocity;
                float dot = Vector2.Dot(rearVel.normalized, toMe.normalized);
                float distance = Vector2.Distance(rb.position, car.transform.position);

                if (dot > 0.7f && distance < car.rearDetectLength)
                    return true;
            }
        }

        return false;
    }

    public bool IsBlockedAtEntry(List<Vector3> path, CarController_v3 car)
    {
        int checkCount = Mathf.Min(5, path.Count);
        for (int i = 0; i < checkCount; i++)
        {
            int idx = Mathf.FloorToInt((i / (float)checkCount) * path.Count);
            Vector3 pos = path[idx];
            Collider2D[] hits = Physics2D.OverlapCircleAll(pos, car.laneMergeCheckRadius, car.carLayer);

            foreach (var col in hits)
            {
                if (col == null || col == car.GetComponent<Collider2D>()) continue;

                CarController_v3 other = col.GetComponent<CarController_v3>();
                if (other == null || other.currentSegment != car.currentSegment) continue;

                float dot = Vector2.Dot(car.transform.up.normalized, other.transform.up.normalized);
                if (dot < 0.75f) continue;

                return true;
            }
        }
        return false;
    }

    public BezierWaypointSegment PickUnoccupiedOrRandomCurve(List<BezierWaypointSegment> segments, CarController_v3 car)
    {
        foreach (var seg in segments)
        {
            Vector3 mid = Vector3.Lerp(seg.transform.position, seg.endNode.transform.position, 0.5f);
            Collider2D[] hits = Physics2D.OverlapCircleAll(mid, car.laneMergeCheckRadius, car.carLayer);

            bool clear = true;
            foreach (var col in hits)
            {
                if (col != null && col != car.GetComponent<Collider2D>())
                {
                    clear = false;
                    break;
                }
            }

            if (clear) return seg;
        }

        return segments[Random.Range(0, segments.Count)];
    }

    public bool EvaluateMergeUnblock(CarController_v3 car)
    {
        Vector2 center = (Vector2)car.transform.position + (Vector2)car.transform.up * 0.5f;
        Collider2D hit = Physics2D.OverlapCircle(center, 0.5f, car.carLayer);

        if (hit == null || hit == car.GetComponent<Collider2D>())
        {
            car.inMergeBlock = false;
            car.currentSpeed = car.currentSpeedLimit;
            return false;
        }

        return true;
    }

    public void TryLaneSwitchOrExitDeadlock(CarController_v3 car)
    {
        Debug.Log($"{car.name} attempting lane switch or deadlock escape (not yet implemented)");
    }
}
