using UnityEngine;
using UnityEngine.SceneManagement;
public class SimulationManager : MonoBehaviour
{
  public static SimulationManager Instance { get; private set; }

  public bool SimulationRunning { get; private set; } = false;

  void Awake()
  {
    if (Instance == null) Instance = this;
    else
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject);
  }
  public void RestartScene()
  {
    Instance = null;         // Clear static reference
    Destroy(gameObject);     // Destroy the current instance
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void StartSimulation()
  {
    SimulationRunning = true;
  }

  public void StopSimulation()
  {
    SimulationRunning = false;
  }

  public void ToggleSimulation()
  {
    SimulationRunning = !SimulationRunning;
  }
}
