using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CurvedWaypointPlacer : MonoBehaviour
{
    public GameObject waypointPrefab;
    public float snapRadius = 0.25f;

    [Header("Naming")]
    public string waypointPrefix = "WP";
    public int namingIndex = 1;
    public bool useDoubleSuffix = false;

    private List<Vector3> clickedPoints = new();
    private float curvatureOffset = 0f;
    private float controlXOffset = 0f;
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

        if (readyToConfirm && e.type == EventType.ScrollWheel)
        {
            if (spaceHeld)
            {
                curvatureOffset += e.delta.y * 0.1f;
                e.Use();
            }
            else if (e.control || e.command)
            {
                controlXOffset += e.delta.y * 0.1f;
                e.Use();
            }
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            clickedPoints.Clear();
            curvatureOffset = 0f;
            controlXOffset = 0f;
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
                    clickedPoints.Add(hit.point);
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
                controlXOffset = 0f;
                readyToConfirm = false;
                curveIndex++;
                e.Use();
                return;
            }
        }

        DrawPreview();
    }

    void PlaceBezierWaypoints()
    {
        Vector3 p0 = clickedPoints[0];
        Vector3 p1 = clickedPoints[1] + new Vector3(controlXOffset, curvatureOffset, 0f);
        Vector3 p2 = clickedPoints[2];

        GameObject start = FindOrCreateWaypoint(p0, $"{curveIndex}.1");
        GameObject end = FindOrCreateWaypoint(p2, $"{curveIndex}.2");

        Link(start, end);

        // Create or find the "Segments" parent
        GameObject parent = GameObject.Find("Segments");
        if (parent == null)
            parent = new GameObject("Segments");

        // Create the segment under the parent
        WaypointNode startNode = start.GetComponent<WaypointNode>();
        int id = startNode.outgoingCurves.Count;
        GameObject segmentObj = new GameObject($"Segment_{startNode.name}_{id}");

        segmentObj.transform.SetParent(parent.transform);
        segmentObj.transform.position = p0;

        var bezier = segmentObj.AddComponent<BezierWaypointSegment>();

        bezier.controlPoint = p1;
        bezier.endNode = end.GetComponent<WaypointNode>();

        // Attach it to the start node
        startNode.outgoingCurves.Add(bezier);

        TryLinkToExisting(p0, start);
        TryLinkFromExisting(p2, end);
    }


    GameObject FindOrCreateWaypoint(Vector3 position, string suffix)
    {
        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            if (Vector3.Distance(wp.transform.position, position) <= snapRadius)
                return wp;
        }

        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(waypointPrefab, transform);
        PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        go.transform.position = position;

        string name = useDoubleSuffix ? $"{waypointPrefix}{suffix}" : $"{waypointPrefix}{namingIndex++}";
        go.name = name;

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
                    if (!existing.nextNodes.Contains(newNode))
                        existing.nextNodes.Add(newNode);

                    if (!newNode.nextNodes.Contains(existing))
                        newNode.nextNodes.Add(existing);
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

            Vector3 control = p1 + new Vector3(controlXOffset, curvatureOffset, 0f);
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
            {
                controlXOffset = newControl.x - p1.x;
                curvatureOffset = newControl.y - p1.y;
            }

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

            Handles.Label(p2 + Vector3.up * 0.3f,
                $"Drag handle or:\nSPACE + Scroll → Y Offset ({curvatureOffset:F2})\nCTRL/CMD + Scroll → X Offset ({controlXOffset:F2})\nDouble-Click to confirm");
        }
    }
}
