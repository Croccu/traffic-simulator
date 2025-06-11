using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPlacer : MonoBehaviour
{
    public GameObject prefabToPlace;
    private GameObject previewObject;
    private bool isPlacing = false;

    public float rotationStep = 45f; // Rotate in 45-degree increments

    void Update()
    {
        if (!isPlacing || previewObject == null)
            return;

        // Follow mouse
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        previewObject.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);

        // Rotate with arrow keys
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
        {
            previewObject.transform.Rotate(0, 0, rotationStep);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.E))
        {
            previewObject.transform.Rotate(0, 0, -rotationStep);
        }

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
    }

    void PlaceItem()
    {
        previewObject = null;
        isPlacing = false;
    }
}
