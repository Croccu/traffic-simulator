using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 20f;

    [Header("Panning Settings")]
    [SerializeField] float panSpeed = 10f;
    [SerializeField] float panBorderThickness = 10f; // pixels from screen edge

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (cam == null)
            return;

        HandleZoom();
        HandlePan();
    }

    void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        if (cam.orthographic)
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
        else
        {
            Vector3 move = transform.forward * scroll * zoomSpeed;
            transform.position += move;
        }
    }

    void HandlePan()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 move = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x >= Screen.width - panBorderThickness)
            move.x += 1f;
        else if (mousePos.x <= panBorderThickness)
            move.x -= 1f;

        if (mousePos.y >= Screen.height - panBorderThickness)
            move.y += 1f;
        else if (mousePos.y <= panBorderThickness)
            move.y -= 1f;

        if (move.sqrMagnitude > 0f)
        {
            Vector3 worldMove = cam.orthographic ? new Vector3(move.x, move.y, 0f) : new Vector3(move.x, 0f, move.y);
            transform.Translate(worldMove.normalized * panSpeed * Time.deltaTime, Space.World);
        }
    }
}
