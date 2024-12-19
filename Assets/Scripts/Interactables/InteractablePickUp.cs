using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;

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
            interactVerb = localizedInteractVerb.GetLocalizedString();
            //interactVerb = LocalizationSettings.StringDatabase.GetLocalizedString($"Items-{interactableItem.Data.Type}", interactableItem.Data.Name);
        }
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                StartCoroutine(InteractCo(interactor));


        }

        IEnumerator InteractCo(GameObject interactor)
        {
            var playerInventory = PlayerInformation.instance.playerInventory;
            interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
            yield return new WaitForSeconds(0.33f);
            

            int space = playerInventory.CheckInventoryHasSpace(pickUpItem, pickupQuantity);
            int finalAmount = pickupQuantity < space ? pickupQuantity : space;
            if (playerInventory.AddItem(pickUpItem, finalAmount, false))
            {
                PlayInteractSound(true);
                Notifications.instance.SetNewNotification("", pickUpItem, finalAmount, NotificationsType.Inventory);
                if (finalAmount < pickupQuantity)
                    pickupQuantity -= finalAmount;
                else 
                {
                    if (pickUpItem.placementGumption != null)
                        PlayerInformation.instance.statHandler.RemoveModifiableModifier(pickUpItem.placementGumption);
                    if (TryGetComponent(out ReplaceObjectOnItemDrop obj))
                        obj.ShowObjects(true);

                    Destroy(gameObject);
                }

            }
            else
            {
                PlayInteractSound(false);
            }

            hasInteracted = false;


            WorldItemManager.instance.RemoveItemFromWorldItemDictionary(interactableItem.Data.Name, 1);
        }


        void PlayInteractSound(bool success)
        {
            if (audioManager.CompareSoundNames("PickUp-" + interactSound))
            {
                audioManager.PlaySound("PickUp-" + interactSound);
            }
        }

    } 
}
