using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabToSpawn;  // Assign this in the Inspector
    private GameObject previewObject;

    public void OnBeginDrag(PointerEventData eventData)
    {
        previewObject = Instantiate(prefabToSpawn);
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        previewObject.transform.position = new Vector3(worldPos.x, worldPos.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        previewObject = null;
    }
}
