using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class SplineRightAngleAnchorSnapper : MonoBehaviour
{
    public PathCreator splinePathCreator;

    public Transform startAnchorPoint;
    public Transform endAnchorPoint;

    public bool snapStart = true;
    public bool snapEnd = true;

    // Kontrollpunktide kaugus anchorist (muuda vastavalt vajadusele)
    public float handleDistance = 0.5f;

    public void Snap()
{
    if (splinePathCreator == null) return;
    var path = splinePathCreator.path;
    if (path == null) return;

    bool changed = false;

    if (snapStart && startAnchorPoint != null)
    {
        Vector3 localStartPos = splinePathCreator.transform.InverseTransformPoint(startAnchorPoint.position);
        path.MovePoint(0, (Vector2)localStartPos);

        // Arvuta suund (näiteks teise anchor poole)
        Vector2 direction = Vector2.right;
        if (endAnchorPoint != null)
        {
            Vector3 localEndPos = splinePathCreator.transform.InverseTransformPoint(endAnchorPoint.position);
            direction = ((Vector2)localEndPos - (Vector2)localStartPos).normalized;
        }

        AdjustControlPointsPerpendicular(path, 0, direction);

        changed = true;
    }

    if (snapEnd && endAnchorPoint != null)
    {
        Vector3 localEndPos = splinePathCreator.transform.InverseTransformPoint(endAnchorPoint.position);
        int endAnchorIndex = path.NumPoints - 1;
        path.MovePoint(endAnchorIndex, (Vector2)localEndPos);

        // Arvuta suund vastupidine start suunale
        Vector2 direction = Vector2.left;
        if (startAnchorPoint != null)
        {
            Vector3 localStartPos = splinePathCreator.transform.InverseTransformPoint(startAnchorPoint.position);
            direction = ((Vector2)localStartPos - (Vector2)localEndPos).normalized;
        }

        AdjustControlPointsPerpendicular(path, endAnchorIndex, direction);

        changed = true;
    }

#if UNITY_EDITOR
    if (changed && !Application.isPlaying)
    {
        EditorUtility.SetDirty(splinePathCreator);
        EditorSceneManager.MarkSceneDirty(splinePathCreator.gameObject.scene);
    }
#endif
}

    void Update()
    {
        Snap();
    }

    // Kontrollpunktide paigutus nii, et nad moodustavad täisnurga (joon alla ja risti teise joonega)
    void AdjustControlPointsPerpendicular(Path path, int anchorIndex, Vector2 direction)
    {
        Vector2 anchorPos = path[anchorIndex];

        // Leia täisnurga vektor (perp. direction)
        Vector2 perpDir = new Vector2(-direction.y, direction.x);

        // Kontrollpunktide kaugus anchorist
        float dist = handleDistance;

        if (anchorIndex - 1 >= 0)
            path.MovePoint(anchorIndex - 1, anchorPos + perpDir * dist);

        if (anchorIndex + 1 < path.NumPoints)
            path.MovePoint(anchorIndex + 1, anchorPos - perpDir * dist);
    }
}