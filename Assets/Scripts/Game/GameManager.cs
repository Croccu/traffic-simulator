using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int carsPassed = 0;
    public int carsDespawned = 0;
    public int carsToDespawn = 0; // ← saad Spawnerist
    public float levelTime = 0f;
    private bool isTiming = false;

    [Header("UI Elements")]
    public GameObject levelCompletePanel;
    public TMP_Text levelCompleteText;

    private void Awake()
    {
        Debug.Log("GameManager Awake");
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (isTiming)
        {
            levelTime += Time.deltaTime;
        }
    }

    public void StartLevelTimer(int totalCars)
    {
        carsToDespawn = totalCars;
        carsDespawned = 0;
        levelTime = 0f;
        isTiming = true;

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }

    public void CarExited()
    {
        carsPassed++;
        Debug.Log("Cars Passed: " + carsPassed);
    }

    public void CarDespawned()
    {
        carsDespawned++;
        Debug.Log("Cars Despawned: " + carsDespawned);

        if (carsDespawned >= carsToDespawn)
        {
            isTiming = false;
            Debug.Log("All cars despawned in " + levelTime + " seconds.");
            ShowScore();
        }
    }

     private void ShowScore()
    {
        float score = CalculateScore(levelTime, carsToDespawn);

        Debug.Log($"Level Complete – Time: {levelTime:F1}s, Score: {score}");

        if (levelCompleteText != null)
        {
            levelCompleteText.text = $"Level Complete\nTime: {levelTime:F1}s\nScore: {score}";
        }

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
    }

    private float CalculateScore(float totalTime, int carCount)
    {
        float maxScorePerCar = 100f; // IGA auto eest kuni 100 punkti
        float timeFactor = Mathf.Max(1f, totalTime); // vältida jagamist nulliga

        float score = (maxScorePerCar * carCount) / timeFactor;
        return Mathf.Round(score);
    }
}