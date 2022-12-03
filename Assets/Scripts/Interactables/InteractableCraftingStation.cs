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
            if (UIScreenManager.instance.CurrentUIScreen() == UIScreenType.PlayerUI)
            {
                OpenCrafting();
                isOpen = true;
            }
        }
        else
        {
            CloseCrafting();
            isOpen = false;
        }
    }

    private void OpenCrafting()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.CraftingStationScreen);
        craftingDisplay.ShowUI();
    }

    private void CloseCrafting()
    {
        UIScreenManager.instance.HideScreens(UIScreenType.CraftingStationScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        craftingDisplay.HideUI();
    }
}
