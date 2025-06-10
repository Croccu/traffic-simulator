using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RegisterLogic : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_Text feedbackText;
    public string targetSceneName; // Field to specify the scene to load

    public void OnRegisterButtonClick()
    {
        string username = usernameField.text;
        string email = emailField.text;
        string password = passwordField.text;

        // Validate input fields
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

        // Simulate registration success
        feedbackText.text = "Registreerimine õnnestus!";
        Debug.Log($"Kasutaja registreeritud: {username}, {email}");

        // Load the target scene
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("Sihtstseeni nimi pole määratud!");
        }
    }

    private bool IsValidEmail(string email)
    {
        // Basic email validation
        return System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}