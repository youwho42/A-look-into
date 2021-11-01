using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCraftingStation : Interactable
{
    bool isOpen;
    CraftingStationDisplayUI craftingDisplay;

    public override void Start()
    {
        base.Start();
        craftingDisplay = CraftingStationDisplayUI.instance;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (!isOpen)
        {
            OpenCrafting();
            isOpen = true;
        }
        else
        {
            CloseCrafting();
            isOpen = false;
        }
    }

    private void OpenCrafting()
    {
        craftingDisplay.ShowUI();
    }

    private void CloseCrafting()
    {

        craftingDisplay.HideUI();
    }
}
