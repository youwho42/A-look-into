using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;

public class InteractablePickUp : Interactable
{
    QI_Item interactableItem;

    QI_Inventory inventoryToAddTo;

    bool addedToInventory;

    public override void Start()
    {
        base.Start();
        interactableItem = GetComponent<QI_Item>();
        interactVerb = interactableItem.Data.Name;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if(InteractCostReward())
            StartCoroutine(InteractCo(interactor));

        
    }

    IEnumerator InteractCo(GameObject interactor)
    {
        interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
        yield return new WaitForSeconds(0.33f);
        PlayInteractSound();

        
        if(PlayerInformation.instance.playerInventory.AddItem(interactableItem.Data, 1))
        {
            if (TryGetComponent(out ReplaceObjectOnItemDrop obj))
            {
                obj.ShowObjects();
            }
            Destroy(gameObject);

        }
            
        hasInteracted = false;

        WorldItemManager.instance.RemoveItemFromWorldItemDictionary(interactableItem.Data.Name, 1);
        
            
    }



    bool InteractCostReward()
    {
        if (playerInformation.playerStats.playerAttributes.GetAttributeValue("PlayerEnergy") >= playerEnergyCost)
        {
            PlayerInformation.instance.playerStats.AddGameEnergy(gameEnergyReward);
            PlayerInformation.instance.playerStats.RemovePlayerEnergy(playerEnergyCost);
            return true;
        }

        NotificationManager.instance.SetNewNotification("You are missing Yellow Bar stuff to pick this up.");
        return false;
    }



    void PlayInteractSound()
    {
        if (audioManager.CompareSoundNames("PickUp-" + interactSound))
        {
            audioManager.PlaySound("PickUp-" + interactSound);
        }
    }

}
