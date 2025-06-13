using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int carsPassed = 0;


    private void Awake()
    {
      Debug.Log("GameManager Awake");
      if (instance == null)
        instance = this;
      else
        Destroy(gameObject);
    }

    // Update is called once per frame
    public void CarExited()
    {
      carsPassed++;
      Debug.Log("Cars Passed: " + carsPassed);
    }
}
