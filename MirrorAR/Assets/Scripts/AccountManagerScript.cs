using UnityEngine;
using UnityEngine.UI;

public class AccountManagerScript : MonoBehaviour
{
    public Text usernameText;

    private void Start()
    {
        string lastLoggedInUsername = PlayerPrefs.GetString("Username");
        if (!string.IsNullOrEmpty(lastLoggedInUsername))
        {
            usernameText.text = "Username: " + lastLoggedInUsername;
        }
        else
        {
            Debug.LogError("No username found in PlayerPrefs.");
        }
    }
}
