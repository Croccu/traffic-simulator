using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] float zoomSpeed = 1f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 20f;

    [Header("Panning Settings")]
    [SerializeField] float panSpeed = 10f;
    [SerializeField] float panBorderThickness = 10f;

    [Header("Movement Bounds")]
    [SerializeField] Vector2 minBounds = new Vector2(-50, -50);
    [SerializeField] Vector2 maxBounds = new Vector2(50, 50);

    [Header("Keyboard Controls")]
    [SerializeField] float keyboardPanSpeed = 10f;

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
        HandleKeyboardPan();
        ClampPosition();
    }

    void HandleZoom()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))
            return;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        float zoomAmount = scroll * 0.5f;

        if (cam.orthographic)
        {
            float newSize = cam.orthographicSize - zoomAmount;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
        else
        {
            Vector3 move = transform.forward * zoomAmount;
            transform.position += move;
        }
    }

    void HandlePan()
    {
        // if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        //     return;

        // float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? 3f : 1f;

        // Vector3 move = Vector3.zero;
        // Vector3 mousePos = Input.mousePosition;

        // if (mousePos.x >= Screen.width - panBorderThickness)
        //     move.x += 1f;
        // else if (mousePos.x <= panBorderThickness)
        //     move.x -= 1f;

        // if (mousePos.y >= Screen.height - panBorderThickness)
        //     move.y += 1f;
        // else if (mousePos.y <= panBorderThickness)
        //     move.y -= 1f;

        // if (move.sqrMagnitude > 0f)
        // {
        //     Vector3 worldMove = cam.orthographic ? new Vector3(move.x, move.y, 0f) : new Vector3(move.x, 0f, move.y);
        //     transform.Translate(worldMove.normalized * panSpeed * speedMultiplier * Time.deltaTime, Space.World);
        // }
    }

    void HandleKeyboardPan()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0f || y != 0f)
        {
            float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? 3f : 1f;
            Vector3 move = cam.orthographic ? new Vector3(x, y, 0f) : new Vector3(x, 0f, y);
            transform.Translate(move.normalized * keyboardPanSpeed * speedMultiplier * Time.deltaTime, Space.World);
        }
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;

        if (cam.orthographic)
        {
            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        }
        else
        {
            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.z = Mathf.Clamp(pos.z, minBounds.y, maxBounds.y); // note: z for depth
        }

        transform.position = pos;
    }
}
