using UnityEngine;

public class InstructionsManager : MonoBehaviour
{
    [Header("Instruction Slides")]
    public GameObject instructionsPanel1;
    public GameObject instructionsPanel2;

    [Header("Explanation Panels (only used in Panel 1)")]
    public GameObject tLightExplanation;
    public GameObject signsExplanation;
    public GameObject homeExplanation;
    public GameObject restartExplanation;
    public GameObject playPauseExplanation;

    private int currentSlide = 1;

    public void OpenInstructions()
    {
        currentSlide = 1;
        instructionsPanel1.SetActive(true);
        instructionsPanel2.SetActive(false);
        HideAllExplanations();
    }

    public void CloseInstructions()
    {
        instructionsPanel1.SetActive(false);
        instructionsPanel2.SetActive(false);
        HideAllExplanations();
    }

    public void ShowExplanation(string type)
    {
        if (currentSlide != 1) return; // Explanations only shown on first slide
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

    public void HideAllExplanations()
    {
        tLightExplanation.SetActive(false);
        signsExplanation.SetActive(false);
        homeExplanation.SetActive(false);
        restartExplanation.SetActive(false);
        playPauseExplanation.SetActive(false);
    }

    public void NextSlide()
    {
        if (currentSlide == 1)
        {
            instructionsPanel1.SetActive(false);
            instructionsPanel2.SetActive(true);
            currentSlide = 2;
        }
    }

    public void PreviousSlide()
    {
        if (currentSlide == 2)
        {
            instructionsPanel2.SetActive(false);
            instructionsPanel1.SetActive(true);
            currentSlide = 1;
        }
    }
}

