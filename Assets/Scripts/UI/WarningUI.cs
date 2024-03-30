using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class WarningUI : MonoBehaviour
{

    public TextMeshProUGUI warningText;
    UIScreenType continueUI;
    public TextMeshProUGUI continueButton;

    UIScreen screen;
    SetButtonSelected backSelect;
    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.WarningUI);
    
        gameObject.SetActive(false);
    }
    public void SetWarning(string warning, UIScreenType continueScreen, string continueButtonText, SetButtonSelected backSelectButton)
    {
        warningText.text = LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", warning);
        continueUI = continueScreen;
        continueButton.text = continueButtonText;
        backSelect = backSelectButton;
    }

    public void Continue()
    {
        if(continueUI == UIScreenType.MainMenuUI)
        {
            UIScreenManager.instance.HideScreenUI();
            LevelManager.instance.LoadTitleScreen();
            Cancel();
            return;
        }
        if (continueUI == UIScreenType.None)
        {
            Application.Quit();
            return;
        }
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayScreenUI(continueUI, true);
        Cancel();
    }

    public void Cancel()
    {
        backSelect.SetSelectedButton();
        gameObject.SetActive(false);
    }
}
