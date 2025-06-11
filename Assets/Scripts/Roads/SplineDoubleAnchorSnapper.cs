using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class SplineDoubleAnchorSnapper : MonoBehaviour
{
    public PathCreator splinePathCreator;

    public Transform startAnchorPoint;
    public Transform endAnchorPoint;

    public bool snapStart = true;
    public bool snapEnd = true;

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
            AdjustControlPoints(path, 0);
            changed = true;
        }

        if (snapEnd && endAnchorPoint != null)
        {
            Vector3 localEndPos = splinePathCreator.transform.InverseTransformPoint(endAnchorPoint.position);
            int endIndex = path.NumPoints - 1;
            path.MovePoint(endIndex, (Vector2)localEndPos);
            AdjustControlPoints(path, endIndex);
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

    void AdjustControlPoints(Path path, int anchorIndex)
    {
        Vector2 anchorPos = path[anchorIndex];
        if (anchorIndex - 1 >= 0)
            path.MovePoint(anchorIndex - 1, Vector2.Lerp(path[anchorIndex - 1], anchorPos, 0.5f));
        if (anchorIndex + 1 < path.NumPoints)
            path.MovePoint(anchorIndex + 1, Vector2.Lerp(path[anchorIndex + 1], anchorPos, 0.5f));
    }
}