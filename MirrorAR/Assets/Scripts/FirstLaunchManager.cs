using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstLaunchManager : MonoBehaviour
{
    private const string firstLaunchKey = "FirstLaunch";

    private void Start()
    {
        // �������� �������� firstLaunch �� PlayerPrefs
        bool isFirstLaunch = PlayerPrefs.GetInt(firstLaunchKey, 1) == 1;

        // ���� ������ ������
        if (isFirstLaunch)
        {
            // ������������� �������� firstLaunch � false
            PlayerPrefs.SetInt(firstLaunchKey, 0);
            PlayerPrefs.Save();

            // ��������� ����� �����������
            SceneManager.LoadScene("Registration");
        }
        else
        {
            // ��������� �������� �����
            SceneManager.LoadScene("MainMenu");
        }
    }
}
