using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Klaxon.Interactable
{
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
            //interactVerb = LocalizationSettings.StringDatabase.GetLocalizedString($"Items-{interactableItem.Data.Type.ToString()}", interactableItem.Data.Name);
            pickUpItem = interactableItem.Data.pickUpItem == null ? interactableItem.Data : interactableItem.Data.pickUpItem;
        }
        public override void SetInteractVerb()
        {
            interactVerb = LocalizationSettings.StringDatabase.GetLocalizedString($"Items-{interactableItem.Data.Type.ToString()}", interactableItem.Data.Name);
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


            if (PlayerInformation.instance.playerInventory.AddItem(pickUpItem, pickupQuantity, false))
            {

                if (TryGetComponent(out ReplaceObjectOnItemDrop obj))
                    obj.ShowObjects(true);

                Notifications.instance.SetNewNotification("", pickUpItem, pickupQuantity, NotificationsType.Inventory);
                //NotificationManager.instance.SetNewNotification($"{pickUpItem.Name} {pickupQuantity}", NotificationManager.NotificationType.Inventory);
                Destroy(gameObject);

            }

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
}
