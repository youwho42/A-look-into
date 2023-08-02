using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableContainer : Interactable
{

    public bool isOpen;
    ContainerInventoryDisplayUI containerUI;
    QI_Inventory inventory;
    public bool isSquirrelBox;

    public override void Start()
    {
        base.Start();
        containerUI = ContainerInventoryDisplayUI.instance;
        if (isSquirrelBox)
        {
            inventory = SquirrelBoxManager.instance.inventory;
        }
        else
        {
            inventory = GetComponent<QI_Inventory>();

            if (inventory == null)
                inventory = GetComponentInParent<QI_Inventory>();
        }
            
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        if (!isOpen) 
            OpenContainer();
        else
            CloseContainer();
    }

    public override void LongInteract(GameObject interactor)
    {
        base.Interact(interactor);

        if (inventory.Stacks.Count > 0)
        {
            NotificationManager.instance.SetNewNotification("Container must be empty to pick up.", NotificationManager.NotificationType.Warning);
            return;
        }
        else
            PickUpContainer();
        
    }

    void PickUpContainer()
    {
        var item = GetComponent<QI_Item>().Data;
        if(PlayerInformation.instance.playerInventory.AddItem(item, 1, false))
        {
            Destroy(gameObject);
        }
        
    }

    private void OpenContainer()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.ContainerScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        containerUI.ShowContainerUI(inventory);
        isOpen = true;
    }

    private void CloseContainer()
    {
        UIScreenManager.instance.HideAllScreens();
        if (LevelManager.instance.HUDBinary == 1)
            UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        containerUI.HideContainerUI();
        isOpen = false;
    }


    public virtual void PlayInteractionSound()
    {
        audioManager.PlaySound(interactSound);
    }
}
