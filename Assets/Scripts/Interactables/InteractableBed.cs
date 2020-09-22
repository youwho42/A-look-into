using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBed : Interactable
{
    DayNightCycle dayNightCycle;

    public GameObject sleepDisplay;

    public override void Start()
    {
        base.Start();

        dayNightCycle = DayNightCycle.instance;

    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        sleepDisplay.SetActive(!sleepDisplay.activeSelf);
        
    }
}
