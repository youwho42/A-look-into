using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBed : Interactable
{
    bool isOpen;
    SleepDisplayUI sleepDisplay;

    public override void Start()
    {
        base.Start();
        sleepDisplay = SleepDisplayUI.instance;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (!isOpen)
        {
            if (UIScreenManager.instance.CurrentUIScreen() == UIScreenType.PlayerUI)
            {
                OpenSleeping();
                isOpen = true;
            }
            
        }
        else
        {
            CloseSleeping();
            isOpen = false;
        }
    }

    private void OpenSleeping()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.SleepScreen);
        UIScreenManager.instance.DisplayPlayerUI();
        sleepDisplay.ShowUI();
    }

    private void CloseSleeping()
    {
        UIScreenManager.instance.HideScreens(UIScreenType.SleepScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        sleepDisplay.HideUI();
    }
}
