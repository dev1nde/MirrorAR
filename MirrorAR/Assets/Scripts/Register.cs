using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button registerButton;
    public Button goToLoginButton;
    public Text errorText;
    public Text infoText;

    void Start()
    {
        registerButton.onClick.AddListener(RegisterButton);
        goToLoginButton.onClick.AddListener(goToLogin);
    }

    void goToLogin()
    {
        SceneManager.LoadScene("Login");
    }

    void RegisterButton()
    {
        if (string.IsNullOrEmpty(usernameInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            infoText.enabled = false;
            errorText.text = "Some fields are empty!";
            errorText.enabled = true;
            return;
        }
        if (PlayerPrefs.HasKey(usernameInput.text))
        {
            errorText.text = "Username already taken!";
            errorText.enabled = true;
        }
        else
        {
            PlayerPrefs.SetString(usernameInput.text, passwordInput.text);
            PlayerPrefs.SetString("Username", usernameInput.text);
            errorText.enabled = false;
            infoText.text = "Registration was successful! Log in to continue.";
            infoText.enabled = true;    
            PlayerPrefs.Save();
    }
    }
}