using UnityEngine;
using TMPro;

public class InteractableTrafficLight : MonoBehaviour
{
    private TrafficLightController controller;
    public GameObject configUI;
    public TMP_InputField redInput, greenInput;

    void Awake()
    {
        controller = GetComponent<TrafficLightController>();
        configUI.SetActive(false);
    }

    void OnMouseDown()
    {
        if (!configUI.activeSelf)
        {
            redInput.text = controller.redDuration.ToString("F1");
            greenInput.text = controller.greenDuration.ToString("F1");
            configUI.SetActive(true);
        }
    }

    public void ApplyChanges()
    {
        if (float.TryParse(redInput.text, out float red))
            controller.redDuration = red;

        if (float.TryParse(greenInput.text, out float green))
            controller.greenDuration = green;

        configUI.SetActive(false);
    }

    public void CancelChanges()
    {
        configUI.SetActive(false);
    }
}
