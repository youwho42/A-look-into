using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MainMenuUI : MonoBehaviour
{

    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.MainMenuUI);
    }
    public void StartNewGame()
    {
        UIScreenManager.instance.DisplayWarning("New Game Warning", UIScreenType.CharacterSelectUI, LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Start"));
        
    }

    public void ViewLoadGameUI()
    {
        UIScreenManager.instance.DisplayLoadGameUI(true, UIScreenType.MainMenuUI);
    }

    public void ViewOptions()
    {
        UIScreenManager.instance.DisplayOptionsUI(true, UIScreenType.MainMenuUI);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
