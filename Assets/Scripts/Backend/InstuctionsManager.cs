using UnityEngine;

public class InstructionsManager : MonoBehaviour
{
    [Header("Main Instructions Panel")]
    public GameObject instructionsPanel;

    [Header("Explanation Panels")]
    public GameObject tLightExplanation;
    public GameObject signsExplanation;
    public GameObject homeExplanation;
    public GameObject restartExplanation;
    public GameObject playPauseExplanation;

    // Open the full instructions panel
    public void OpenInstructions()
    {
        instructionsPanel.SetActive(true);
        HideAllExplanations();
    }

    // Close the instructions panel entirely
    public void CloseInstructions()
    {
        instructionsPanel.SetActive(false);
        HideAllExplanations();
    }

    // Show only one explanation at a time
    public void ShowExplanation(string type)
    {
        HideAllExplanations();

        switch (type)
        {
            case "traffic":
                tLightExplanation.SetActive(true);
                break;
            case "signs":
                signsExplanation.SetActive(true);
                break;
            case "home":
                homeExplanation.SetActive(true);
                break;
            case "restart":
                restartExplanation.SetActive(true);
                break;
            case "play":
                playPauseExplanation.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown explanation type: " + type);
                break;
        }
    }

    // Helper to disable all explanation panels
    public void HideAllExplanations()
    {
        tLightExplanation.SetActive(false);
        signsExplanation.SetActive(false);
        homeExplanation.SetActive(false);
        restartExplanation.SetActive(false);
        playPauseExplanation.SetActive(false);
    }
}

