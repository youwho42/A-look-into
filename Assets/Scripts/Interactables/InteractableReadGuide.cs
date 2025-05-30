using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableReadGuide : Interactable
    {
        public QI_ItemData readableItem;

        public override void Start()
        {
            base.Start();

        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);

            StartCoroutine(InteractCo(interactor));


        }

        IEnumerator InteractCo(GameObject interactor)
        {
            interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
            yield return new WaitForSeconds(0.33f);
            PlayInteractSound();

            if (!PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Contains(readableItem))
            {
                PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Add(readableItem);
                Notifications.instance.SetNewLargeNotification(null, readableItem, null, NotificationsType.Compendium);
                //NotificationManager.instance.SetNewNotification($"{readableItem.Name} guide found", NotificationManager.NotificationType.Compendium);
                GameEventManager.onGuideCompediumUpdateEvent.Invoke();
            }



            Destroy(gameObject);
            hasInteracted = false;
            WorldItemManager.instance.RemoveItemFromWorldItemDictionary(readableItem.Name, 1);

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