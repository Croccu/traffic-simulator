using UnityEngine;

public class SignPanelController : MonoBehaviour
{
    public GameObject signPanel; // Assign this in the Inspector

    // Toggle: open if closed, close if open
    public void ToggleSignPanel()
    {
        signPanel.SetActive(!signPanel.activeSelf);
    }

    // Optional: open only
    public void ShowSignPanel()
    {
        signPanel.SetActive(true);
    }

    // Optional: close only
    public void HideSignPanel()
    {
        signPanel.SetActive(false);
    }
}

