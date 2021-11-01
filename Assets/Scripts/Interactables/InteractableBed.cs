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
            sleepDisplay.ShowUI();
            isOpen = true;
        }
        else
        {
            sleepDisplay.HideUI();
            isOpen = false;
        }
    }

   
}
