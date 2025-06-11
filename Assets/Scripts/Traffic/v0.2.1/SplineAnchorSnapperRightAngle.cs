using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class SplineAnchorSnapperRightAngle : MonoBehaviour
{
    public PathCreator splinePathCreator;
    public Transform anchorPoint;
    public bool snapStart = true;

    // Kui sul on mingi teine punkt, mille suunas 90° nurka soovid (nt ühenduv tee algus/lõpp)
    // Lisa see optional
    public Transform referencePoint;

    public float handleDistance = 0.5f;

    public void Snap()
    {
        if (splinePathCreator == null || anchorPoint == null) return;
        Path path = splinePathCreator.path;
        if (path == null) return;

        Vector3 localAnchorPos = splinePathCreator.transform.InverseTransformPoint(anchorPoint.position);
        int anchorIndex = snapStart ? 0 : path.NumPoints - 1;

        path.MovePoint(anchorIndex, (Vector2)localAnchorPos);

        // Arvutame suuna referencePointi suunas, kui see on määratud
        Vector2 direction = Vector2.right; // Vaikimisi suund

        if (referencePoint != null)
        {
            Vector3 localRefPos = splinePathCreator.transform.InverseTransformPoint(referencePoint.position);
            direction = ((Vector2)localRefPos - (Vector2)localAnchorPos).normalized;
        }

        AdjustControlPointsPerpendicular(path, anchorIndex, direction);

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

    // Täisnurga kontrollpunktide seadmine (ristsuund)
    void AdjustControlPointsPerpendicular(Path path, int anchorIndex, Vector2 direction)
    {
        Vector2 anchorPos = path[anchorIndex];

        Vector2 perpDir = new Vector2(-direction.y, direction.x);

        if (anchorIndex - 1 >= 0)
            path.MovePoint(anchorIndex - 1, anchorPos + perpDir * handleDistance);

        if (anchorIndex + 1 < path.NumPoints)
            path.MovePoint(anchorIndex + 1, anchorPos - perpDir * handleDistance);
    }
}