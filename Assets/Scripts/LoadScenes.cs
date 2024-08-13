using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScenes : MonoBehaviour
{
    [SerializeField] private GameObject _sceneMenu;
    [SerializeField] private Button _openMenuButton;
    [SerializeField] private Button _closeMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        _sceneMenu.SetActive(false);
        _openMenuButton.onClick.AddListener(OpenMenu);
        _closeMenuButton.onClick.AddListener(CloseMenu);
    }

    private void OpenMenu()
    {
        _sceneMenu.SetActive(true);
        _openMenuButton.enabled = false;
    }

    private void CloseMenu()
    {
        _sceneMenu.SetActive(false);
        _openMenuButton.enabled = true;
        Debug.Log("Close Menu");
    }

    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == sceneName)
            return;
        

      SceneManager.LoadScene(sceneName);
    }

}
