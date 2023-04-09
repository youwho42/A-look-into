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

    private void OpenContainer()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.ContainerScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        containerUI.ShowContainerUI(inventory);
        isOpen = true;
    }

    private void CloseContainer()
    {
        UIScreenManager.instance.HideScreens(UIScreenType.ContainerScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        containerUI.HideContainerUI();
        isOpen = false;
    }


    public virtual void PlayInteractionSound()
    {
        audioManager.PlaySound(interactSound);
    }
}
