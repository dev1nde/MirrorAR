using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad;

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
