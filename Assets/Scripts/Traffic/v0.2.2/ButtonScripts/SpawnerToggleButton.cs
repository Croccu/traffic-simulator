using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnerToggleButton : MonoBehaviour
{
    [Header("UI Elements")]
    public Button toggleButton;
    public TextMeshProUGUI buttonText;

    [Header("Spawners to Control")]
    public BezierSplineSpawner[] spawners;

    private bool isSpawning = false;

    void Start()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleSpawners);
        }

        UpdateButtonText();
    }

    void ToggleSpawners()
    {
        isSpawning = !isSpawning;

        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                if (isSpawning)
                    spawner.StartSpawning();
                else
                    spawner.StopSpawning();
            }
        }

        UpdateButtonText();
        Debug.Log("Spawner toggled. Spawning: " + isSpawning);
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isSpawning ? "Peata" : "Käivita";
        }
    }
}
