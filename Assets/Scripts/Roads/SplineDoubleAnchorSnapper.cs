using UnityEngine;

public class SplineDoubleAnchorSnapper : MonoBehaviour
{
    public PathCreator splinePathCreator;

    public Transform startAnchorPoint;  // Anchor for spline start
    public Transform endAnchorPoint;    // Anchor for spline end

    // Helper method to adjust control points near an anchor point on the spline
    private void AdjustControlPoints(Path path, int anchorIndex)
    {
        Vector2 anchorPos = path[anchorIndex];
        // Adjust control point before anchor
        if (anchorIndex - 1 >= 0)
            path.MovePoint(anchorIndex - 1, Vector2.Lerp(path[anchorIndex - 1], anchorPos, 0.5f));
        // Adjust control point after anchor
        if (anchorIndex + 1 < path.NumPoints)
            path.MovePoint(anchorIndex + 1, Vector2.Lerp(path[anchorIndex + 1], anchorPos, 0.5f));
    }

    public void SnapBothEnds()
    {
        if (splinePathCreator == null)
        {
            Debug.LogWarning("Assign splinePathCreator.");
            return;
        }

        var path = splinePathCreator.path;
        if (path == null)
        {
            Debug.LogWarning("Spline Path is null.");
            return;
        }

        if (startAnchorPoint != null)
        {
            Vector3 localStartPos = splinePathCreator.transform.InverseTransformPoint(startAnchorPoint.position);
            path.MovePoint(0, (Vector2)localStartPos);
            AdjustControlPoints(path, 0);  // Adjust control points near start anchor
        }

        if (endAnchorPoint != null)
        {
            Vector3 localEndPos = splinePathCreator.transform.InverseTransformPoint(endAnchorPoint.position);
            int endIndex = path.NumPoints - 1;
            path.MovePoint(endIndex, (Vector2)localEndPos);
            AdjustControlPoints(path, endIndex);  // Adjust control points near end anchor
        }
    }

    void Start()
    {
        SnapBothEnds();
    }
}