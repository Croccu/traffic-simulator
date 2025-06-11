using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class LoginLogic : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text feedbackText;

    [Header("API Settings")]
    public string usersDataUrl = "YOUR_GET_SCRIPT_URL";
    public string targetScene = "Main";

    private UserData[] allUsers;

    public void OnLoginButtonClick()
    {
        StartCoroutine(CheckLoginCredentials());
    }

    IEnumerator CheckLoginCredentials()
    {
        // First download all user data
        UnityWebRequest www = UnityWebRequest.Get(usersDataUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            feedbackText.text = "Connection error. Try again later.";
            yield break;
        }

        // Parse JSON data
        try
        {
            string jsonString = "{\"users\":" + www.downloadHandler.text + "}";
            allUsers = JsonHelper.FromJson<UserData>(jsonString);
        }
        catch
        {
            feedbackText.text = "Data format error.";
            yield break;
        }

        // Check credentials
        string username = usernameField.text.Trim();
        string password = passwordField.text;

        var matchedUser = allUsers.FirstOrDefault(user =>
            user.username == username &&
            user.password == password);

        if (matchedUser != null)
        {
            feedbackText.text = "Login successful!";
            PlayerPrefs.SetString("LoggedInUser", username);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            feedbackText.text = "Invalid username or password";
        }
    }

    [System.Serializable]
    private class UserData
    {
        public string username;
        public string email;
        public string password;
    }

    // Helper class for array JSON parsing
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.users;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] users;
        }
    }
}
