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
    public TMP_Text feedbackText;

    [Header("Scene & API Settings")]
    public string targetSceneName; // Scene to load after success
    [TextArea]
    public string googleSheetPostUrl = "https://script.google.com/macros/s/AKfycbx-IMMNDIzt6dC0Kqjmwh8vlKKXlEllN2_b9CUsqozSbqwlNMWmovaEFuKoJs766Zf0-Q/exec"; // Google Apps Script Web App URL

    public void OnRegisterButtonClick()
    {
        string username = usernameField.text.Trim();
        string email = emailField.text.Trim();
        string password = passwordField.text;

        // Validation checks
        if (string.IsNullOrEmpty(username))
        {
            feedbackText.text = "Kasutajanimi ei tohi olla tühi.";
            return;
        }

        if (!IsValidEmail(email))
        {
            feedbackText.text = "E-posti aadress ei ole sobiv.";
            return;
        }

        if (password.Length < 6)
        {
            feedbackText.text = "Parool peab olema vähemalt 6 tähemärki pikk.";
            return;
        }

        // Start coroutine to send data to Google Sheets
        StartCoroutine(SendDataToGoogleSheet(username, email, password));
    }

    // Add this to your existing Unity code:

    [Header("Debug Settings")]
    public bool showDebugLogs = true;

    private IEnumerator SendDataToGoogleSheet(string username, string email, string password)
    {
        // Create JSON data
        var jsonData = new RegisterData
        {
            username = username,
            email = email,
            password = password
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
            www.timeout = 10; // 10 seconds timeout

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                if (showDebugLogs)
                    Debug.Log("Response: " + www.downloadHandler.text);

                feedbackText.text = "Registreerimine õnnestus!";
                yield return new WaitForSeconds(1.5f); // Show success message

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
                Debug.LogError($"Error: {www.error}\nResponse: {www.downloadHandler.text}");
            }
        }
    }

    [System.Serializable]
    private class ErrorResponse
    {
        public string result;
        public string message;
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
    }
}
