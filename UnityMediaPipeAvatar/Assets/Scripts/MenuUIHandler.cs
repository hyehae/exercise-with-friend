using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUIHandler : MonoBehaviour
{
    public GameObject infoPanel;
    public Button howButton;
    public Button closeButton;
    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        infoPanel.SetActive(false);

        howButton.onClick.AddListener(ActivatePanel);
        closeButton.onClick.AddListener(ClosePanel);
        startButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ActivatePanel()
    {
        infoPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        infoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
