using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableContainer : Interactable
{

    public bool isOpen;
    ContainerInventoryDisplayUI containerUI;
    QI_Inventory inventory;
    public override void Start()
    {
        base.Start();
        containerUI = ContainerInventoryDisplayUI.instance;
        inventory = GetComponent<QI_Inventory>();
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        if (!isOpen) 
        {
            OpenContainer();
            isOpen = true;
        }
        else
        {
            CloseContainer();
            isOpen = false;
        }
    }

    private void OpenContainer()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.ContainerScreen);
        containerUI.ShowContainerUI(inventory);
    }

    private void CloseContainer()
    {
        UIScreenManager.instance.HideScreens(UIScreenType.ContainerScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        containerUI.HideContainerUI();
    }


    public virtual void PlayInteractionSound()
    {
        audioManager.PlaySound(interactSound);
    }
}
