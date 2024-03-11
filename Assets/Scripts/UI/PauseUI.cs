using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class PauseUI : MonoBehaviour
{

    [HideInInspector]
    public bool inPauseMenu;
    [SerializeField]
    private TextMeshProUGUI pauseSaveAlertText;
    public Transform unstuckPlayerPosition;

    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.PauseUI);
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
    public void UnPause()
    {
        UIScreenManager.instance.SetPauseScreen(false);
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
        var currentClipInfo = PlayerInformation.instance.playerAnimator.GetCurrentAnimatorClipInfo(0);
        if (currentClipInfo[0].clip.name != "Craft")
        {
            SavingLoading.instance.SaveGame();
            pauseSaveAlertText.text = "Saved";
        }
        else
        {
            Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Cannot Save"), null, 0, NotificationsType.Warning);
        }
            
    }

    public void ViewLoadGameUI()
    {
        UIScreenManager.instance.DisplayLoadGameUI(true, UIScreenType.PauseUI);
    }

    public void UnstuckPlayer()
    {
        PlayerInformation.instance.player.position = unstuckPlayerPosition.position;
        PlayerInformation.instance.currentTilePosition.position = PlayerInformation.instance.currentTilePosition.GetCurrentTilePosition(unstuckPlayerPosition.position);
        PlayerInformation.instance.playerController.currentLevel = (int)unstuckPlayerPosition.position.z - 1;
        UIScreenManager.instance.SetPauseScreen(false);
    }
}
