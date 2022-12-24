using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class InteractableCraftingStation : Interactable
{
    bool isOpen;
    CraftingStationDisplayUI craftingDisplay;
    QI_CraftingHandler craftingHandler;
    public QI_CraftingRecipeDatabase recipeDatabase;
    public QI_Inventory selfInventory;
    public override void Start()
    {
        base.Start();
        craftingDisplay = CraftingStationDisplayUI.instance;
        craftingHandler = GetComponent<QI_CraftingHandler>();
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
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        selfInventory = selfInventory != null ? selfInventory : playerInformation.playerInventory;
        craftingDisplay.ShowUI(craftingHandler, recipeDatabase, selfInventory);
    }

    private void CloseCrafting()
    {
        UIScreenManager.instance.HideScreens(UIScreenType.CraftingStationScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        craftingDisplay.HideUI();
    }
}
