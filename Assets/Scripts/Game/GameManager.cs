using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int carsPassed = 0;
    public int carsDespawned = 0;
    public int carsToDespawn = 0; // ← saad Spawnerist
    public float levelTime = 0f;
    private bool isTiming = false;

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
        float score = CalculateScore(levelTime);
        Debug.Log("Final Score: " + score);
        // Siin võiksid kutsuda välja UI-teavituse jne.
    }

    private float CalculateScore(float time)
    {
        float startScore = 100f;
        float decayRate = 1f; // mitu punkti kaotab 1 sekundi jooksul

        float score = startScore - (time * decayRate);
        return Mathf.Max(0f, Mathf.Round(score)); // ei lase minna alla 0
    }
}