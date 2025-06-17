using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CurvedWaypointPlacer : MonoBehaviour
{
    public GameObject waypointPrefab;
    public float snapRadius = 1f;

    private List<Vector3> clickedPoints = new();
    private float curvatureOffset = 0f;
    private int curveIndex = 1;
    private bool readyToConfirm = false;
    private bool spaceHeld = false;

    private int handleId = -1;
    private Vector3 lastControlPoint = Vector3.zero;

    void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.activeGameObject != gameObject)
            return;

        Event e = Event.current;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space)
            spaceHeld = true;
        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Space)
            spaceHeld = false;

        if (readyToConfirm && e.type == EventType.ScrollWheel && spaceHeld)
        {
            curvatureOffset += e.delta.y * 0.1f;
            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            clickedPoints.Clear();
            curvatureOffset = 0f;
            readyToConfirm = false;
            e.Use();
            return;
        }

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (GUIUtility.hotControl == handleId)
                return;

            if (clickedPoints.Count < 3)
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider != null)
                {
                    Vector3 snapped = TrySnapToExistingWaypoint(hit.point, clickedPoints.Count == 0 || clickedPoints.Count == 2);
                    clickedPoints.Add(snapped);
                    if (clickedPoints.Count == 3)
                        readyToConfirm = true;
                    e.Use();
                    return;
                }
            }
            else if (clickedPoints.Count == 3 && readyToConfirm)
            {
                clickedPoints.Add(Vector3.zero);
                e.Use();
                return;
            }
            else if (clickedPoints.Count == 4 && readyToConfirm)
            {
                PlaceBezierWaypoints();
                clickedPoints.Clear();
                curvatureOffset = 0f;
                readyToConfirm = false;
                curveIndex++;
                e.Use();
                return;
            }
        }

        DrawPreview();
    }

    Vector3 TrySnapToExistingWaypoint(Vector3 point, bool allowSnap)
    {
        if (!allowSnap) return point;

        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            if (Vector3.Distance(wp.transform.position, point) <= snapRadius)
                return wp.transform.position;
        }
        return point;
    }

    void PlaceBezierWaypoints()
    {
        Vector3 p0 = clickedPoints[0];
        Vector3 p1 = clickedPoints[1] + Vector3.up * curvatureOffset;
        Vector3 p2 = clickedPoints[2];

        GameObject start = InstantiateWaypoint(p0, $"{curveIndex}.1");
        GameObject end = InstantiateWaypoint(p2, $"{curveIndex}.2");

        Link(start, end);

        var bezier = start.AddComponent<BezierWaypointSegment>();
        bezier.controlPoint = p1;
        bezier.endNode = end.GetComponent<WaypointNode>();

        TryLinkToExisting(p0, start);
        TryLinkFromExisting(p2, end);
    }

    GameObject InstantiateWaypoint(Vector3 pos, string label)
    {
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(waypointPrefab, transform);
        go.transform.position = pos;
        go.name = $"Curve{label}";
        return go;
    }

    void Link(GameObject from, GameObject to)
    {
        var a = from.GetComponent<WaypointNode>();
        var b = to.GetComponent<WaypointNode>();
        if (a != null && b != null && a != b && !a.nextNodes.Contains(b))
            a.nextNodes.Add(b);
    }

    void TryLinkToExisting(Vector3 point, GameObject newFirst)
    {
        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            if (Vector3.Distance(wp.transform.position, point) <= snapRadius)
            {
                var existing = wp.GetComponent<WaypointNode>();
                var newNode = newFirst.GetComponent<WaypointNode>();

                if (existing != null && newNode != null && existing != newNode)
                {
                    if (!existing.nextNodes.Contains(newNode))
                        existing.nextNodes.Add(newNode);
                }
            }
        }
    }

    void TryLinkFromExisting(Vector3 point, GameObject newLast)
    {
        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            if (Vector3.Distance(wp.transform.position, point) <= snapRadius)
            {
                var existing = wp.GetComponent<WaypointNode>();
                var newNode = newLast.GetComponent<WaypointNode>();

                if (existing != null && newNode != null && existing != newNode)
                {
                    // This is where existing node gets a new .nextNode = newLast
                    if (!existing.nextNodes.Contains(newNode))
                        existing.nextNodes.Add(newNode);
                }
            }
        }
    }


    void DrawPreview()
    {
        if (clickedPoints.Count == 1)
        {
            Handles.color = Color.white;
            Handles.DrawSolidDisc(clickedPoints[0], Vector3.forward, 0.1f);
        }
        else if (clickedPoints.Count == 2)
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(clickedPoints[0], clickedPoints[1]);
        }
        else if (clickedPoints.Count >= 3)
        {
            Vector3 p0 = clickedPoints[0];
            Vector3 p1 = clickedPoints[1];
            Vector3 p2 = clickedPoints[2];

            Vector3 control = p1 + Vector3.up * curvatureOffset;
            lastControlPoint = control;

            Handles.color = new Color(0f, 1f, 1f, 0.1f);
            Handles.DrawSolidDisc(control, Vector3.forward, 0.3f);

            Handles.color = Color.cyan;
            handleId = GUIUtility.GetControlID(FocusType.Passive);
            Vector3 newControl = Handles.FreeMoveHandle(
                handleId,
                control,
                0.2f,
                Vector3.zero,
                Handles.SphereHandleCap
            );

            if (newControl != control)
                curvatureOffset = newControl.y - p1.y;

            Handles.color = Color.green;
            Vector3 prev = p0;
            for (float t = 0; t <= 1f; t += 0.05f)
            {
                Vector3 pt = Mathf.Pow(1 - t, 2) * p0 +
                             2 * (1 - t) * t * control +
                             Mathf.Pow(t, 2) * p2;
                Handles.DrawLine(prev, pt);
                prev = pt;
            }

            Handles.Label(p2 + Vector3.up * 0.3f, "Drag handle or SPACE + Scroll\nClick to confirm");
        }
    }
}
