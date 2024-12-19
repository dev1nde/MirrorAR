using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstLaunchManager : MonoBehaviour
{
    private const string firstLaunchKey = "FirstLaunch";

    private void Start()
    {
        // Получаем значение firstLaunch из PlayerPrefs
        bool isFirstLaunch = PlayerPrefs.GetInt(firstLaunchKey, 1) == 1;

        // Если первый запуск
        if (isFirstLaunch)
        {
            // Устанавливаем значение firstLaunch в false
            PlayerPrefs.SetInt(firstLaunchKey, 0);
            PlayerPrefs.Save();

            // Загружаем сцену регистрации
            SceneManager.LoadScene("Registration");
        }
        else
        {
            // Загружаем основную сцену
            SceneManager.LoadScene("MainMenu");
        }
    }
}
