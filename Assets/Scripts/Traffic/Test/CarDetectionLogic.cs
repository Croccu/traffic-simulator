using System.Collections.Generic;
using UnityEngine;

public class CarDetectionLogic : MonoBehaviour
{
    public void AdjustSpeedBasedOnCarAhead(CarController_v3 car)
    {
        Vector2 center = (Vector2)car.transform.position + (Vector2)car.transform.up * (car.detectionLength * 0.5f);
        Vector2 size = new Vector2(car.detectionWidth, car.detectionLength);
        float angle = car.transform.eulerAngles.z;

        Collider2D hit = Physics2D.OverlapCapsule(center, size, CapsuleDirection2D.Vertical, angle, LayerMask.GetMask("Car"));

        if (hit != null && hit != car.GetComponent<Collider2D>())
        {
            CarController_v3 carAhead = hit.GetComponent<CarController_v3>();
            if (carAhead != null && carAhead.currentSegment != car.currentSegment) return;

            float dot = Vector2.Dot(car.transform.up.normalized, carAhead.transform.up.normalized);
            if (dot < 0.75f) return;

            float dist = Vector2.Distance(car.transform.position, hit.transform.position);
            if (dist < car.stopDistance)
            {
                car.currentSpeed = 0f;
            }
            else
            {
                float t = (dist - car.stopDistance) / (car.detectionLength - car.stopDistance);
                car.currentSpeed = Mathf.Lerp(0f, car.currentSpeedLimit, t);
            }
        }
        else
        {
            car.currentSpeed = car.currentSpeedLimit;
        }
    }

    public bool IsRearCarTooClose(CarController_v3 car)
    {
        Vector2 backCenter = (Vector2)car.transform.position - (Vector2)car.transform.up * (car.rearDetectLength * 0.5f);
        Vector2 rearSize = new Vector2(car.rearDetectWidth, car.rearDetectLength);
        float angle = car.transform.eulerAngles.z;

        Collider2D rearHit = Physics2D.OverlapCapsule(backCenter, rearSize, CapsuleDirection2D.Vertical, angle, LayerMask.GetMask("Car"));
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
            Collider2D[] hits = Physics2D.OverlapCircleAll(pos, car.laneMergeCheckRadius, LayerMask.GetMask("Car"));

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
            Collider2D[] hits = Physics2D.OverlapCircleAll(mid, car.laneMergeCheckRadius, LayerMask.GetMask("Car"));

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
        Collider2D hit = Physics2D.OverlapCircle(center, 0.5f, LayerMask.GetMask("Car"));

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
        // Stubbed placeholder
        Debug.Log($"{car.name} attempting lane switch or deadlock escape (not yet implemented)");
    }
}
