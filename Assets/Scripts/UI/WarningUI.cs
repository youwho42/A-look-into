using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class WarningUI : MonoBehaviour
{

    public TextMeshProUGUI warningText;
    UIScreenType continueUI;
    

    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void SetWarning(string warning, UIScreenType continueScreen)
    {
        warningText.text = LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", warning);
        continueUI = continueScreen;
    }

    public void Continue()
    {
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayScreenUI(continueUI, true);
        Cancel();
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}
