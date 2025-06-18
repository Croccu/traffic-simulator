using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPlacer : MonoBehaviour
{
    public GameObject prefabToPlace;
    private GameObject previewObject;
    private bool isPlacing = false;

    public float rotationStep = 45f;

    void Update()
    {
        if (!isPlacing || previewObject == null)
            return;

        // Follow mouse
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        previewObject.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);

        // Rotate with Q / Left Arrow
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
            previewObject.transform.Rotate(0, 0, rotationStep);

        // Rotate with E / Right Arrow
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.E))
            previewObject.transform.Rotate(0, 0, -rotationStep);

        // Place with left mouse button (LMB)
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            PlaceItem();
        }

        // Cancel with right mouse button (RMB)
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    public void StartPlacing()
    {
        if (isPlacing) return;

        previewObject = Instantiate(prefabToPlace);
        isPlacing = true;

        // Optionally enable detection zone for placement preview
        Transform detection = previewObject.transform.Find("DetectionZone");
        if (detection != null)
            detection.gameObject.SetActive(true);
    }

    void PlaceItem()
    {
        if (previewObject != null)
        {
            // Disable detection zone after placing
            Transform detection = previewObject.transform.Find("DetectionZone");
            if (detection != null)
                detection.gameObject.SetActive(false);

            previewObject = null;
        }

        isPlacing = false;
    }

    void CancelPlacement()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        isPlacing = false;
        Debug.Log("Placement cancelled.");
    }
}
