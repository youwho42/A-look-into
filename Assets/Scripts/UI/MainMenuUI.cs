using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    UIScreen screen;
    [HideInInspector]
    public List<Button> allButtons = new List<Button>();

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.MainMenuUI);
        allButtons = GetComponentsInChildren<Button>().ToList();
    }
    
    public void StartNewGame()
    {
        var backSelect = GetComponent<SetButtonSelected>();
        UIScreenManager.instance.DisplayWarning("New Game Warning", UIScreenType.CharacterSelectUI, LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Start"), backSelect);
        
    }

    public void ViewLoadGameUI()
    {
        UIScreenManager.instance.DisplayLoadGameUI(true, UIScreenType.MainMenuUI);
    }

    public void ViewOptions()
    {
        UIScreenManager.instance.DisplayOptionsUI(true, UIScreenType.MainMenuUI);
    }

    public void ViewCredits()
    {
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayIngameUI(UIScreenType.CreditsUI, true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
