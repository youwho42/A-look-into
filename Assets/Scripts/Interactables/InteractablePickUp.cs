using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;

public class InteractablePickUp : Interactable
{
    QI_Item interactableItem;
    QI_ItemData pickUpItem;
    QI_Inventory inventoryToAddTo;

    bool addedToInventory;
    public int pickupQuantity = 1;
    public override void Start()
    {
        base.Start();
        interactableItem = GetComponent<QI_Item>();
        interactVerb = interactableItem.Data.Name;
        pickUpItem = interactableItem.Data.pickUpItem == null ? interactableItem.Data : interactableItem.Data.pickUpItem;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if(/*InteractCostReward() && */UIScreenManager.instance.canChangeUI)
            StartCoroutine(InteractCo(interactor));

        
    }

    IEnumerator InteractCo(GameObject interactor)
    {
        interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
        yield return new WaitForSeconds(0.33f);
        PlayInteractSound();

        
        if(PlayerInformation.instance.playerInventory.AddItem(pickUpItem, pickupQuantity, false))
        {
            if (TryGetComponent(out ReplaceObjectOnItemDrop obj))
            {
                obj.ShowObjects();
            }
            
            NotificationManager.instance.SetNewNotification($"{pickUpItem.Name} {pickupQuantity}", NotificationManager.NotificationType.Inventory);
            Destroy(gameObject);

        }
            
        hasInteracted = false;

        WorldItemManager.instance.RemoveItemFromWorldItemDictionary(interactableItem.Data.Name, 1);
        
            
    }



    //bool InteractCostReward()
    //{
    //    if (playerInformation.playerStats.playerAttributes.GetAttributeValue("Bounce") >= playerEnergyCost)
    //    {
    //        PlayerInformation.instance.playerStats.AddGameEnergy(gameEnergyReward);
    //        PlayerInformation.instance.playerStats.RemovePlayerEnergy(playerEnergyCost);
    //        return true;
    //    }

    //    NotificationManager.instance.SetNewNotification("You are missing Bounce to pick this up.");
    //    return false;
    //}



    void PlayInteractSound()
    {
        if (audioManager.CompareSoundNames("PickUp-" + interactSound))
        {
            audioManager.PlaySound("PickUp-" + interactSound);
        }
    }

}
