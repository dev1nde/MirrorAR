using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;
    public Text errorText;

    void Start()
    {
        goToRegisterButton.onClick.AddListener(goToRegister);
    }

    public void login()
    {
        string savedUsername = PlayerPrefs.GetString(usernameInput.text);
        string savedPassword = PlayerPrefs.GetString(passwordInput.text);

        if (string.IsNullOrEmpty(usernameInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            errorText.text = "Some fields are empty!";
            errorText.enabled = true;
            return;
        }

        if (savedUsername == usernameInput.text && savedPassword == passwordInput.text)
        {
            PlayerPrefs.SetString("Username", usernameInput.text);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Menu");
        }
        else if (savedUsername == usernameInput.text)
        {
            errorText.text = "Invalid password!";
            errorText.enabled = true;
        }
        else
        {
            errorText.text = "User not found!";
            errorText.enabled = true;
        }
    }

    void goToRegister()
    {
        SceneManager.LoadScene("Register");
    }
}
