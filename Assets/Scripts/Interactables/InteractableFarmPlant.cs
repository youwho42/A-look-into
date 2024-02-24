using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
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
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                StartCoroutine(InteractCo(interactor));


        }

        IEnumerator InteractCo(GameObject interactor)
        {
            interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
            yield return new WaitForSeconds(0.33f);
            PlayInteractSound();

            foreach (var item in pickUpItem.harvestedItems)
            {
                int amount = pickUpItem.GetAmount(item.amountVariance, item.minMaxAmount);
                if (PlayerInformation.instance.playerInventory.AddItem(item.harvestedItem, amount, false))
                {
                    Notifications.instance.SetNewNotification("", item.harvestedItem, amount, NotificationsType.Inventory);
                    //NotificationManager.instance.SetNewNotification($"{item.harvestedItem.Name} {amount}", NotificationManager.NotificationType.Inventory);
                }
                else
                {
                    for (int i = 0; i < amount; i++)
                    {
                        Instantiate(item.harvestedItem, transform.position, Quaternion.identity);
                    }

                }

            }
            plantingArea.plantFreeLocations.Add(transform.position);
            plantingArea.plantUsedLocations.Remove(transform.position);
            plantingArea.harvestablePlants.Remove(GetComponent<PlantLife>());
            plantingArea.CheckForHarvestable();
            hasInteracted = false;
            WorldItemManager.instance.RemoveItemFromWorldItemDictionary(interactableItem.Data.Name, 1);
            Destroy(gameObject);
        }


        void PlayInteractSound()
        {
            if (audioManager.CompareSoundNames("PickUp-" + interactSound))
            {
                audioManager.PlaySound("PickUp-" + interactSound);
            }
        }
    }

}