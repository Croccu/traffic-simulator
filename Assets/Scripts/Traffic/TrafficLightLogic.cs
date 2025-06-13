using UnityEngine;
using System.Collections;

public class TrafficLightLogic : MonoBehaviour
{
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;

    void Start()
    {
        StartCoroutine(SwitchLights());
    }

    IEnumerator SwitchLights()
    {
        while (true)
        {
            SetActiveLight(redLight);
            yield return new WaitForSeconds(5f);

            SetActiveLight(yellowLight);
            yield return new WaitForSeconds(2f);

            SetActiveLight(greenLight);
            yield return new WaitForSeconds(5f);
        }
    }

    void SetActiveLight(GameObject active)
    {
        redLight.SetActive(false);
        yellowLight.SetActive(false);
        greenLight.SetActive(false);

        active.SetActive(true);
    }
}
