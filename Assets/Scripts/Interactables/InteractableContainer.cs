using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableContainer : Interactable
{

    bool isOpen;

    public override void Start()
    {
        base.Start();
        
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
    private void CloseContainer()
    {
        gameObject.GetComponent<ContainerInventoryDisplay>().HideContainerUI();
    }
    private void OpenContainer()
    {
        
        gameObject.GetComponent<ContainerInventoryDisplay>().ShowContainerUI();
    }

    public virtual void PlayInteractionSound()
    {
        audioManager.PlaySound(interactSound);
    }
}
