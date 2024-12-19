using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq; 

public class AutoCompleteController : MonoBehaviour
{
    public InputField searchInputField;
    public Dropdown sceneDropdown; 
    public Text noResultsText; 
    private List<string> sceneNames = new List<string>();

    private void Start()
    {
        searchInputField.onValueChanged.AddListener(OnSearchInputValueChanged);
        noResultsText.gameObject.SetActive(false);
        FillSceneDropdown();
        sceneDropdown.gameObject.SetActive(false);
    }

    private void FillSceneDropdown()
    {
        sceneDropdown.ClearOptions();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            sceneNames.Add(sceneName);
        }
        sceneNames.Remove("SampleScene");
        List<Dropdown.OptionData> options = sceneNames.Select(sceneName => new Dropdown.OptionData(sceneName)).ToList();
        sceneDropdown.AddOptions(options);
    }

    public void OnSearchInputValueChanged(string inputText)
    {
        List<string> filteredSceneNames = sceneNames.Where(sceneName => sceneName.ToLower().Contains(inputText.ToLower())).ToList();
        sceneDropdown.ClearOptions();
        List<Dropdown.OptionData> options = filteredSceneNames.Select(sceneName => new Dropdown.OptionData(sceneName)).ToList();
        sceneDropdown.AddOptions(options);
        noResultsText.gameObject.SetActive(options.Count == 0);
        if (options.Count == 0)
        {
            sceneDropdown.gameObject.SetActive(false);           }
        else
        {
            sceneDropdown.gameObject.SetActive(true);
        }

    }

    public void LoadSelectedScene()
    {
        int selectedIndex = sceneDropdown.value; 
        string selectedSceneName = sceneNames[selectedIndex]; 
        SceneManager.LoadScene(selectedSceneName);
    }

}
