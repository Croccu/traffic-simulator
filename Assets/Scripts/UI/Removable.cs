using UnityEngine;

public class Removable : MonoBehaviour
{
    void OnMouseOver()
    {
        if ((Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl)) ||
            (Input.GetMouseButtonDown(1) && !FindObjectOfType<ItemPlacer>().IsPlacing()))
        {
            Destroy(gameObject);
        }
    }
}