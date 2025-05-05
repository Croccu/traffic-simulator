using UnityEngine;

public class IntersectionControl : MonoBehaviour
{
    public static bool stopAtIntersection = false; // Global flag to stop cars at intersections

    private void OnMouseDown()
    {
        // Toggle the stopAtIntersection flag when the object is clicked
        stopAtIntersection = !stopAtIntersection;
        Debug.Log($"Intersection control toggled: {stopAtIntersection}");
    }
}