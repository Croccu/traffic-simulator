using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class SplineAnchorSnapper : MonoBehaviour
{
    public PathCreator splinePathCreator;
    public Transform anchorPoint;  // Anchor to snap to
    public bool snapStart = true;  // Snap start if true, else snap end

    public void Snap()
    {
        if (splinePathCreator == null || anchorPoint == null) return;

        var path = splinePathCreator.path;
        if (path == null) return;

        Vector3 localPos = splinePathCreator.transform.InverseTransformPoint(anchorPoint.position);
        int pointIndex = snapStart ? 0 : path.NumPoints - 1;

        path.MovePoint(pointIndex, (Vector2)localPos);
        AdjustControlPoints(path, pointIndex);

#if UNITY_EDITOR
        if (!Application.isPlaying)
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