using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseUI : MonoBehaviour
{

    [HideInInspector]
    public bool inPauseMenu;
    [SerializeField]
    private TextMeshProUGUI pauseSaveAlertText;

    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        pauseSaveAlertText.text = "Pause";
    }
    public void SetPause(bool state)
    {
        inPauseMenu = state;
        RealTimeDayNightCycle.instance.isPaused = state;
        Time.timeScale = state ? 0.0f : 1.0f;
    }

    // Called from buttons set in inspector
    public void ViewOptions()
    {
        UIScreenManager.instance.DisplayOptionsUI(true, UIScreenType.PauseUI);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        LevelManager.instance.LoadTitleScreen();
    }

    public void SaveGame()
    {
        SavingLoading.instance.SaveGame();
        pauseSaveAlertText.text = "Saved";
    }

    public void ViewLoadGameUI()
    {
        UIScreenManager.instance.DisplayLoadGameUI(true, UIScreenType.PauseUI);
    }
}
