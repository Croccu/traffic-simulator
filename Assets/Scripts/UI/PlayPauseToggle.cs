using UnityEngine;
using UnityEngine.UI;

public class PlayPauseToggle : MonoBehaviour
{
    public GameObject panelToToggle;
    public Button playPauseButton;

    private void Start()
    {
        playPauseButton.onClick.AddListener(TogglePanel);
    }

    private void TogglePanel()
    {
        if (panelToToggle != null)
        {
            bool isActive = panelToToggle.activeSelf;
            panelToToggle.SetActive(!isActive);
        }
    }
}
