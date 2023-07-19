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

    public override void LongInteract(GameObject interactor)
    {
        base.Interact(interactor);

        if (selfInventory.Stacks.Count > 0)
        {
            NotificationManager.instance.SetNewNotification("Container must be empty to pick up.", NotificationManager.NotificationType.Warning);
            return;
        }
        else if (craftingHandler.Queues.Count > 0)
        {
            NotificationManager.instance.SetNewNotification("Must not be crafting to pick up.", NotificationManager.NotificationType.Warning);
            return;
        }
        else
            PickUpCraftingStation();

    }

    void PickUpCraftingStation()
    {
        var item = GetComponent<QI_Item>().Data;
        if (PlayerInformation.instance.playerInventory.AddItem(item, 1, false))
        {
            Destroy(gameObject);
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
        UIScreenManager.instance.HideAllScreens();
        //UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        craftingDisplay.HideUI();
    }
}
