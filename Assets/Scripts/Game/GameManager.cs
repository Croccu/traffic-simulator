using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Gameplay Tracking")]
    public int carsPassed = 0;
    public int carsDespawned = 0;
    public int carsToDespawn = 0;
    public float levelTime = 0f;
    private bool isTiming = false;

    [Header("UI Elements")]
    public GameObject levelCompletePanel;
    public TMP_Text levelCompleteText;

    private void Awake()
    {
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

    // 👉 Kutsutakse iga spawneri poolt
    public void AddToSpawnTarget(int count)
    {
        if (!isTiming)
        {
            levelTime = 0f;
            carsDespawned = 0;
            isTiming = true;
        }

        carsToDespawn += count;
        Debug.Log($"Added {count} cars. Total to despawn: {carsToDespawn}");
    }

    public void CarDespawned()
    {
        carsDespawned++;
        Debug.Log($"Car despawned. Total: {carsDespawned}/{carsToDespawn}");

        if (carsDespawned >= carsToDespawn)
        {
            isTiming = false;
            ShowScore();
        }
    }

    public void CarExited()
    {
        carsPassed++;
        Debug.Log("Cars Passed: " + carsPassed);
    }

    private void ShowScore()
    {
        float score = CalculateScore(levelTime, carsToDespawn);

        Debug.Log($"Level Complete – Time: {levelTime:F1}s, Score: {score}");

        if (levelCompleteText != null)
        {
            levelCompleteText.text =
                "LEVEL COMPLETE\n\n" +
                $"Time: {levelTime:F1} seconds\n" +
                $"Cars: {carsToDespawn}\n" +
                $"Score: {score}";
        }

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
    }

    private float CalculateScore(float totalTime, int carCount)
    {
        float maxScorePerCar = 100f; // Võib muuta suuremaks, kui soovid
        float timeFactor = Mathf.Max(1f, totalTime); // Väldib jagamist 0-ga

        float score = (maxScorePerCar * carCount) / timeFactor;
        return Mathf.Round(score);
    }
}