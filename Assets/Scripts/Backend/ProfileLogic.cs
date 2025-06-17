using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class ProfileLogic : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameField;
    public TMP_InputField emailField;
    public TMP_InputField newPasswordField; // this is always shown, but empty by default
    public TMP_Text feedbackText;
    public GameObject feedbackPanel;
    public TMP_Text editButtonLabel;

    [Header("API Settings")]
    public string updateProfileUrl = "https://script.google.com/macros/s/YOUR_SCRIPT_ID/exec";

    private bool isEditMode = false;

    void Start()
    {
        string username = PlayerPrefs.GetString("LoggedInUser", "");
        string email = PlayerPrefs.GetString("LoggedInEmail", "");

        usernameField.text = username;
        emailField.text = email;
        newPasswordField.text = "";

        usernameField.interactable = false;
        emailField.interactable = false;
        newPasswordField.interactable = false;

        editButtonLabel.text = "Muuda andmeid";
        feedbackPanel.SetActive(false);
    }

    public void ToggleEditMode()
    {
        isEditMode = !isEditMode;

        usernameField.interactable = isEditMode;
        emailField.interactable = isEditMode;
        newPasswordField.interactable = isEditMode;

        if (isEditMode)
        {
            newPasswordField.text = "";
            editButtonLabel.text = "Kinnita";
        }
        else
        {
            editButtonLabel.text = "Muuda andmeid";
            StartCoroutine(SubmitUpdatedProfile());
        }
    }

    private IEnumerator SubmitUpdatedProfile()
    {
        string newUsername = usernameField.text.Trim();
        string newEmail = emailField.text.Trim();
        string newPassword = newPasswordField.text.Trim();
        string oldEmail = PlayerPrefs.GetString("LoggedInEmail", "");

        // Basic checks
        if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newEmail))
        {
            feedbackText.text = "Kasutajanimi ja e-post ei tohi olla tühjad";
            feedbackPanel.SetActive(true);
            yield break;
        }

        if (!IsValidEmail(newEmail))
        {
            feedbackText.text = "E-posti aadress ei ole sobiv";
            feedbackPanel.SetActive(true);
            yield break;
        }

        if (!string.IsNullOrEmpty(newPassword) && newPassword.Length < 6)
        {
            feedbackText.text = "Parool peab olema vähemalt 6 tähemärki pikk";
            feedbackPanel.SetActive(true);
            yield break;
        }

        var updateData = new ProfileUpdateRequest
        {
            profileUpdate = true,
            oldEmail = oldEmail,
            newUsername = newUsername,
            newEmail = newEmail,
            newPassword = string.IsNullOrEmpty(newPassword) ? null : newPassword
        };

        string json = JsonUtility.ToJson(updateData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest www = new UnityWebRequest(updateProfileUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            feedbackText.text = "Ühenduse viga";
            feedbackPanel.SetActive(true);
            yield break;
        }

        var response = JsonUtility.FromJson<UpdateResponse>(www.downloadHandler.text);

        if (response.result == "success")
        {
            PlayerPrefs.SetString("LoggedInUser", newUsername);
            PlayerPrefs.SetString("LoggedInEmail", newEmail);

            feedbackText.text = "Andmed edukalt uuendatud!";
        }
        else
        {
            feedbackText.text = response.message ?? "Tundmatu viga";
        }

        feedbackPanel.SetActive(true);
    }

    private bool IsValidEmail(string email)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    [System.Serializable]
    private class ProfileUpdateRequest
    {
        public bool profileUpdate;
        public string oldEmail;
        public string newUsername;
        public string newEmail;
        public string newPassword;
    }

    [System.Serializable]
    private class UpdateResponse
    {
        public string result;
        public string message;
    }
}






