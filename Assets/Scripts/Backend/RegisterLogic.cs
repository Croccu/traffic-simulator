using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class RegisterLogic : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField countryField;
    public TMP_InputField cityField;
    public TMP_InputField birthdateField; // oodatud formaat: dd/mm/yyyy
    public TMP_Text feedbackText;
    public GameObject feedbackPanel;

    [Header("Scene & API Settings")]
    public string targetSceneName;
    [TextArea]
    public string googleSheetPostUrl = "https://script.google.com/macros/s/..."; // sinu URL

    [Header("Debug Settings")]
    public bool showDebugLogs = true;

    public void OnRegisterButtonClick()
    {
        string username = usernameField.text.Trim();
        string email = emailField.text.Trim();
        string password = passwordField.text;
        string country = countryField.text.Trim();
        string city = cityField.text.Trim();
        string birthdate = birthdateField.text.Trim();

        // Kontrollid
        if (string.IsNullOrEmpty(username))
        {
            feedbackText.text = "Kasutajanimi ei tohi olla tühi";
            feedbackPanel.SetActive(true);
            return;
        }

        if (!IsValidEmail(email))
        {
            feedbackText.text = "E-posti aadress ei ole sobiv";
            feedbackPanel.SetActive(true);
            return;
        }

        if (password.Length < 6)
        {
            feedbackText.text = "Parooli pikkus peab olema vähemalt 6 tähemärki";
            feedbackPanel.SetActive(true);
            return;
        }

        if (string.IsNullOrEmpty(country) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(birthdate))
        {
            feedbackText.text = "Palun täida kõik väljad (riik, linn, sünnikuupäev)";
            feedbackPanel.SetActive(true);
            return;
        }

        StartCoroutine(SendDataToGoogleSheet(username, email, password, country, city, birthdate));
    }

    private IEnumerator SendDataToGoogleSheet(string username, string email, string password, string country, string city, string birthdate)
    {
        var jsonData = new RegisterData
        {
            username = username,
            email = email,
            password = password,
            country = country,
            city = city,
            birthdate = birthdate
        };

        string json = JsonUtility.ToJson(jsonData);

        if (showDebugLogs)
            Debug.Log("Sending data: " + json);

        using (UnityWebRequest www = new UnityWebRequest(googleSheetPostUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.timeout = 10;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                if (showDebugLogs)
                    Debug.Log("Response: " + www.downloadHandler.text);

                feedbackText.text = "Registreerimine õnnestus!";
                feedbackPanel.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                feedbackPanel.SetActive(false);

                if (!string.IsNullOrEmpty(targetSceneName))
                    SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                string errorMessage = "Viga registreerimisel";

                try
                {
                    var errorResponse = JsonUtility.FromJson<ErrorResponse>(www.downloadHandler.text);
                    if (!string.IsNullOrEmpty(errorResponse.message))
                        errorMessage += ": " + errorResponse.message;
                }
                catch { }

                feedbackText.text = errorMessage;
                feedbackPanel.SetActive(true);
                Debug.LogError($"Error: {www.error}\nResponse: {www.downloadHandler.text}");
            }
        }
    }

    private bool IsValidEmail(string email)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$"
        );
    }

    [System.Serializable]
    private class RegisterData
    {
        public string username;
        public string email;
        public string password;
        public string country;
        public string city;
        public string birthdate;
    }

    [System.Serializable]
    private class ErrorResponse
    {
        public string result;
        public string message;
    }
}


