using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VersatileSceneLoader : MonoBehaviour
{
    public Text sceneNameText;

    public void VersatileSceneLoad()
    {
        string sceneName = sceneNameText.text;
        SceneManager.LoadScene(sceneName);
    }
}
