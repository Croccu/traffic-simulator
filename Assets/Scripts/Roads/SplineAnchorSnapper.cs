using UnityEngine;

public class SplineAnchorSnapper : MonoBehaviour
{
    public PathCreator splinePathCreator;
    public Transform anchorPoint; // Set to intersection anchor transform

    public enum SnapPoint { Start, End }
    public SnapPoint snapAt = SnapPoint.Start;

    public void Snap()
    {
        if (splinePathCreator == null || anchorPoint == null)
        {
            Debug.LogWarning("Assign splinePathCreator and anchorPoint before snapping.");
            return;
        }

        var path = splinePathCreator.path;
        if (path == null)
        {
            Debug.LogWarning("Spline Path is null.");
            return;
        }

        // Convert anchor world position to spline local space
        Vector3 localPos = splinePathCreator.transform.InverseTransformPoint(anchorPoint.position);

        if (snapAt == SnapPoint.Start)
        {
            path.MovePoint(0, (Vector2)localPos);
        }
        else // Snap at End
        {
            path.MovePoint(path.NumPoints - 1, (Vector2)localPos);
        }
    }

    // Optional: snap automatically on start
    void Start()
    {
        Snap();
    }
}