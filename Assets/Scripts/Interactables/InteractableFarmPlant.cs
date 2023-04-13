using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableFarmPlant : Interactable
{
    QI_Item interactableItem;
    PlantedItemData pickUpItem;
    public PlantingArea plantingArea;
    
    public override void Start()
    {
        base.Start();
        interactableItem = GetComponent<QI_Item>();
        interactVerb = interactableItem.Data.Name;
        if (interactableItem.Data.GetType() == typeof(PlantedItemData))
            pickUpItem = interactableItem.Data as PlantedItemData;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (/*InteractCostReward() && */UIScreenManager.instance.canChangeUI)
            StartCoroutine(InteractCo(interactor));


    }

    IEnumerator InteractCo(GameObject interactor)
    {
        interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
        yield return new WaitForSeconds(0.33f);
        PlayInteractSound();

        foreach (var item in pickUpItem.harvestedItems)
        {
            if (PlayerInformation.instance.playerInventory.AddItem(item.harvestedItem, item.harvestedAmount, false))
            {
                float total = PlayerInformation.instance.playerInventory.GetStock(item.harvestedItem.Name);
                NotificationManager.instance.SetNewNotification($"{item.harvestedItem.Name} {total}", NotificationManager.NotificationType.Inventory);
            }
            else
            {
                for (int i = 0; i < item.harvestedAmount; i++)
                {
                    Instantiate(item.harvestedItem, transform.position, Quaternion.identity);
                }
                
            }

        }
        plantingArea.plantFreeLocations.Add(transform.position);
        plantingArea.plantUsedLocations.Remove(transform.position);
        plantingArea.CheckForHarvestable();
        Destroy(gameObject);
        hasInteracted = false;
        WorldItemManager.instance.RemoveItemFromWorldItemDictionary(interactableItem.Data.Name, 1);

    }


    void PlayInteractSound()
    {
        if (audioManager.CompareSoundNames("PickUp-" + interactSound))
        {
            audioManager.PlaySound("PickUp-" + interactSound);
        }
    }
}
