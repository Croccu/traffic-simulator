using UnityEngine;

public class UiButtonController : MonoBehaviour
{
    [Header("Sign Panel Toggle")]
    public GameObject signPanel;

    [Header("Traffic Light Panel Toggle")]
    public GameObject trafficLightPanel; // ✅ NEW PANEL

    [Header("Play/Pause Toggle")]
    public GameObject playImage;
    public GameObject pauseImage;
    private static bool isPlaying = false;

    public static bool IsPlaying => isPlaying;

    void Awake()
    {
        isPlaying = false; // Reset play state on scene load
    }
    public void ToggleSignPanel()
    {
        if (signPanel != null)
        {
            signPanel.SetActive(!signPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("UiButtonController: signPanel is not assigned!");
        }
    }

    public void ToggleTrafficLightPanel() // ✅ NEW METHOD
    {
        if (trafficLightPanel != null)
        {
            trafficLightPanel.SetActive(!trafficLightPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("UiButtonController: trafficLightPanel is not assigned!");
        }
    }

    public void TogglePlayPause()
    {
        isPlaying = !isPlaying;

        if (playImage != null && pauseImage != null)
        {
            playImage.SetActive(!isPlaying);
            pauseImage.SetActive(isPlaying);
        }
        else
        {
            Debug.LogWarning("UiButtonController: One or both icon GameObjects not assigned!");
        }

        Debug.Log(isPlaying ? "Playing..." : "Paused.");
    }
}
