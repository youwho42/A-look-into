using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBed : Interactable
{
    bool isOpen;
    public bool facingRight;
    public NavigationNode navigationNode;
    SleepDisplayUI sleepDisplay;

    public override void Start()
    {
        base.Start();
        sleepDisplay = SleepDisplayUI.instance;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        //if (!isOpen)
        //{
        var screen = LevelManager.instance.HUDBinary == 0 ? UIScreenType.None : UIScreenType.PlayerUI;
        if (UIScreenManager.instance.CurrentUIScreen() == screen)
        {
            OpenSleeping();
            isOpen = true;
        }
            
        //}
        //else
        //{
        //    CloseSleeping();
        //    isOpen = false;
        //}
    }

    private void OpenSleeping()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.SleepScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        sleepDisplay.ShowUI();
    }

    //private void CloseSleeping()
    //{
    //    UIScreenManager.instance.HideScreens(UIScreenType.SleepScreen);
    //    UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
    //    sleepDisplay.HideUI();
    //}
}
