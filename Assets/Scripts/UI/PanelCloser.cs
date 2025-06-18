using System.Collections.Generic;
using UnityEngine;

public class PanelCloser : MonoBehaviour
{
    [Header("Panels to Check and Close")]
    public List<GameObject> panelsToClose;
    
    public void CloseOpenPanels()
    {
        foreach (GameObject panel in panelsToClose)
        {
            if (panel != null && panel.activeSelf)
            {
                panel.SetActive(false);
                Debug.Log($"Closed panel: {panel.name}");
            }
        }
    }
}
