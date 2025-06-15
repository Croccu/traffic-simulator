using UnityEngine;
using UnityEngine.UI;

public class UiButtonController : MonoBehaviour
{
    [Header("Sign Panel Toggle")]
    public GameObject signPanel; // Assign the UI panel to show/hide

    [Header("Play/Pause Button Toggle")]
    public Image playPauseButtonImage;  // Assign the button's Image component
    public Sprite playSprite;           // Sprite to show when in "play" state
    public Sprite pauseSprite;          // Sprite to show when in "pause" state

    private bool isPlaying = false;

    // Toggles the sign panel on or off
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

    // Toggles play/pause state and updates the icon
    public void TogglePlayPause()
    {
        isPlaying = !isPlaying;

        if (playPauseButtonImage != null)
        {
            playPauseButtonImage.sprite = isPlaying ? pauseSprite : playSprite;
        }
        else
        {
            Debug.LogWarning("UiButtonController: playPauseButtonImage is not assigned!");
        }

        // Optional: Your game logic (e.g., time scale)
        if (isPlaying)
        {
            Debug.Log("Playing...");
            // Time.timeScale = 1;
        }
        else
        {
            Debug.Log("Paused.");
            // Time.timeScale = 0;
        }
    }
}


