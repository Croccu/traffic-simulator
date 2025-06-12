using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

public class UserDisplay : MonoBehaviour
{
    public TMP_Text welcomeText;
    public TMP_Text emailText;
    public TMP_Text timestampText;

    [Header("API Settings")]
    public string usersDataUrl = "https://script.google.com/macros/s/AKfycbx-IMMNDIzt6dC0Kqjmwh8vlKKXlEllN2_b9CUsqozSbqwlNMWmovaEFuKoJs766Zf0-Q/exec";

    void Start()
    {
        string username = PlayerPrefs.GetString("LoggedInUser", "Guest");
        if (username == "Guest")
        {
            welcomeText.text = "No information available. Please log in.";
            emailText.text = "";
            timestampText.text = "";
            Debug.LogWarning("No user logged in. Displaying default message.");
            return;
        }
        StartCoroutine(FetchAndDisplayUserInfo(username));
    }

    IEnumerator FetchAndDisplayUserInfo(string username)
    {
        UnityWebRequest www = UnityWebRequest.Get(usersDataUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            welcomeText.text = "Error loading user info.";
            emailText.text = "";
            timestampText.text = "";
            yield break;
        }
        Debug.Log("JSON data received: " + www.downloadHandler.text);

        string jsonString = "{\"users\":" + www.downloadHandler.text + "}";
        UserData[] allUsers;
        try
        {
            allUsers = JsonHelper.FromJson<UserData>(jsonString);
        }
        catch
        {
            welcomeText.text = "Data format error.";
            emailText.text = "";
            timestampText.text = "";
            yield break;
        }

        var user = allUsers.FirstOrDefault(u => u.username == username);
        if (user != null)
        {
            welcomeText.text = "Konto nimi: " + user.username;
            emailText.text = "Email: " + user.email;
            timestampText.text = "Konto loodud: " + user.timestamp;
        }
        else
        {
            welcomeText.text = "User info not found.";
            emailText.text = "Ei leidnud emaili.";
            timestampText.text = "Ei leinud loomise aega.";
        }
    }

    [System.Serializable]
    private class UserData
    {
        public string username;
        public string email;
        public string password;
        public string timestamp;
    }

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