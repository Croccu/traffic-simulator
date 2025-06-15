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
    public GameObject feedbackPanel; // <<< UUS: paneel tagasiside jaoks (nt punase taustaga)

    [Header("Scene & API Settings")]
    public string targetSceneName;
    [TextArea]
    public string googleSheetPostUrl = "https://script.google.com/macros/s/..."; // sinu URL

    public void OnRegisterButtonClick()
    {
        string username = usernameField.text.Trim();
        string email = emailField.text.Trim();
        string password = passwordField.text;

        // Validation checks
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

        StartCoroutine(SendDataToGoogleSheet(username, email, password));
    }

    [Header("Debug Settings")]
    public bool showDebugLogs = true;

    private IEnumerator SendDataToGoogleSheet(string username, string email, string password)
    {
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
            www.timeout = 10;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                if (showDebugLogs)
                    Debug.Log("Response: " + www.downloadHandler.text);

                feedbackText.text = "Registreerimine õnnestus!";
                feedbackPanel.SetActive(true); // <<< Näita edu paneeli
                yield return new WaitForSeconds(1.5f);
                feedbackPanel.SetActive(false); // <<< Peida paneel

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
                feedbackPanel.SetActive(true); // <<< Näita error paneeli
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

