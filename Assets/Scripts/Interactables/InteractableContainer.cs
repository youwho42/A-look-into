using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableContainer : Interactable
{

    bool isOpen;
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
            containerUI.ShowContainerUI(inventory);
            isOpen = true;
        }
        else
        {
            containerUI.HideContainerUI();
            isOpen = false;
        }
    }
    

    public virtual void PlayInteractionSound()
    {
        audioManager.PlaySound(interactSound);
    }
}
