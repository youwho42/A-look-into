using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{

    public GameObject quitUI;
    public GameObject displayUI;
    public Toggle vSync;
    public Toggle fullscreen;


    private void Start()
    {
        quitUI.SetActive(false);
        displayUI.SetActive(false);
        fullscreen.isOn = Screen.fullScreen;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !displayUI.activeSelf)
            ToggleOptionsUI();
            
    }

    public void ToggleOptionsUI()
    {
        quitUI.SetActive(!quitUI.activeSelf);
        
    }

    public void ToggleDisplayUI()
    {
        displayUI.SetActive(!displayUI.activeSelf);
        quitUI.SetActive(!displayUI.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVSync()
    {
        if (vSync.isOn)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = fullscreen.isOn;
    }
}
