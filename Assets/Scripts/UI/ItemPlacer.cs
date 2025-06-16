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

        // Rotate
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
            previewObject.transform.Rotate(0, 0, rotationStep);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.E))
            previewObject.transform.Rotate(0, 0, -rotationStep);

        // Place with left-click
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            PlaceItem();
        }
    }

    public void StartPlacing()
    {
        if (isPlacing) return;

        previewObject = Instantiate(prefabToPlace);
        isPlacing = true;
        Transform detection = previewObject.transform.Find("DetectionZone");
        if (detection != null)
            detection.gameObject.SetActive(true);
    }

    void PlaceItem()
    {
        if (previewObject != null)
        {
            Transform detection = previewObject.transform.Find("DetectionZone");
            if (detection != null)
                detection.gameObject.SetActive(false);
        }

        previewObject = null;
        isPlacing = false;
    }
}
