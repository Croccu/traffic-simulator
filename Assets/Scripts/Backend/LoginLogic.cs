using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginLogic : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text feedbackText;
    public GameObject feedbackPanel;

    [Header("API Settings")]
    public string loginApiUrl = "https://script.google.com/macros/s/YOUR_SCRIPT_ID/exec";
    public string targetScene = "Menu";

    public void OnLoginButtonClick()
    {
        string username = usernameField.text.Trim();
        string password = passwordField.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Palun sisesta kasutajanimi ja parool";
            feedbackPanel.SetActive(true);
            return;
        }

        StartCoroutine(SendLoginRequest(username, password));
    }

    private IEnumerator SendLoginRequest(string username, string password)
    {
        LoginRequest loginData = new LoginRequest
        {
            loginRequest = true,
            username = username,
            password = password
        };

        string json = JsonUtility.ToJson(loginData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest www = new UnityWebRequest(loginApiUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            feedbackText.text = "Ühenduse viga.";
            feedbackPanel.SetActive(true);
            Debug.LogError(www.error);
            yield break;
        }

        LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

        if (response.result == "success")
        {
            feedbackText.text = "Sisselogimine õnnestus!";
            feedbackPanel.SetActive(true);

            PlayerPrefs.SetString("LoggedInUser", response.username);
            PlayerPrefs.SetString("LoggedInEmail", response.email);
            PlayerPrefs.SetString("LoggedInCity", response.city ?? "");
            PlayerPrefs.SetString("LoggedInCountry", response.country ?? "");

            yield return new WaitForSeconds(1.5f);
            feedbackPanel.SetActive(false);
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            feedbackText.text = response.message ?? "Vale kasutajanimi või parool";
            feedbackPanel.SetActive(true);
        }
    }

    [System.Serializable]
    private class LoginRequest
    {
        public bool loginRequest;
        public string username;
        public string password;
    }

    [System.Serializable]
    private class LoginResponse
    {
        public string result;
        public string message;
        public string username;
        public string email; 
        public string city;
        public string country;
    }
}






