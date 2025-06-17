using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelInfo
{
    public string levelName;
    public Sprite previewSprite;
    public string sceneName; // Scene to load when Play is pressed
}

public class LevelDetailsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject levelDetailsPanel;
    public Image previewImage;
    public TMP_Text levelHeaderText;

    [Header("Level Data")]
    public LevelInfo[] levels;

    private int currentIndex = 0;

    void Start()
    {
        levelDetailsPanel.SetActive(false); // Hide panel at start
    }

    public void OpenLevelDetails(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, levels.Length - 1);
        UpdateUI();
        levelDetailsPanel.SetActive(true);
    }

    public void CloseLevelDetails()
    {
        levelDetailsPanel.SetActive(false);
    }

    public void ShowNextLevel()
    {
        currentIndex = (currentIndex + 1) % levels.Length;
        UpdateUI();
    }

    public void ShowPreviousLevel()
    {
        currentIndex = (currentIndex - 1 + levels.Length) % levels.Length;
        UpdateUI();
    }

    public void PlayCurrentLevel()
    {
        string sceneToLoad = levels[currentIndex].sceneName;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name is missing for this level.");
        }
    }

    private void UpdateUI()
    {
        previewImage.sprite = levels[currentIndex].previewSprite;
        levelHeaderText.text = levels[currentIndex].levelName;
    }
}
