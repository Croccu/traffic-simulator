using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GoogleSheetsReader : MonoBehaviour
{
    public string csvUrl = "https://script.google.com/macros/s/AKfycbx-OmYnK-Lm33KdGzglL_cDVLAKHPITEasAmlxdDu9NdH1Pe60t_I_NTxq1LqgTgHg0YA/exec";

    void Start()
    {
        StartCoroutine(DownloadCSV());
    }

    IEnumerator DownloadCSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(csvUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string csvText = www.downloadHandler.text;
            Debug.Log(csvText);
            // Parse CSV here and use data
        }
        else
        {
            Debug.LogError("Error downloading CSV: " + www.error);
        }
    }
}
