using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.CreditsUI);
        gameObject.SetActive(false);
    }
    public void CloseCredits()
    {
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayIngameUI(UIScreenType.MainMenuUI, true);
    }
}
