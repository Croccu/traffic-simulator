using UnityEngine;

public class UiButtonController : MonoBehaviour
{
    [Header("Sign Panel Toggle")]
    public GameObject signPanel;

    [Header("Play/Pause Toggle")]
    public GameObject playImage;
    public GameObject pauseImage;
    private bool isPlaying = false;

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

        if (isPlaying)
        {
            Debug.Log("Playing...");

        }
        else
        {
            Debug.Log("Paused.");

        }
    }
}



