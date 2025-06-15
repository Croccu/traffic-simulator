using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class DateFormatter : MonoBehaviour
{
    public TMP_InputField inputField;
    private bool isUpdating = false;

    void Start()
    {
        inputField.onValueChanged.AddListener(FormatInput);
    }

    void FormatInput(string input)
    {
        if (isUpdating) return;
        isUpdating = true;

        
        string digitsOnly = Regex.Replace(input, @"\D", "");

        
        if (digitsOnly.Length > 2)
        {
            digitsOnly = digitsOnly.Insert(2, "/");
        }
        if (digitsOnly.Length > 5)
        {
            digitsOnly = digitsOnly.Insert(5, "/");
        }

        
        if (digitsOnly.Length > 10)
        {
            digitsOnly = digitsOnly.Substring(0, 10);
        }

        inputField.text = digitsOnly;
        isUpdating = false;
    }

    void Update()
    {
        
        if (inputField.caretPosition != inputField.text.Length)
        {
            inputField.caretPosition = inputField.text.Length;
        }
    }
}

