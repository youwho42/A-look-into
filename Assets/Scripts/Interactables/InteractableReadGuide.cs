using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableReadGuide : Interactable
    {
        //public QI_ItemData readableItem;

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

            if (!PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Contains(playerGuide))
            {
                PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Add(playerGuide);
                Notifications.instance.SetNewLargeNotification(null, playerGuide, null, NotificationsType.Compendium);
                BallPersonMessageDisplayUI.instance.ShowSimpleMessage(playerGuide.localizedName.GetLocalizedString(), playerGuide.localizedDescription.GetLocalizedString(), null);
                UIScreenManager.instance.DisplayIngameUI(UIScreenType.BallPersonDialogueUI, true);
                //NotificationManager.instance.SetNewNotification($"{readableItem.Name} guide found", NotificationManager.NotificationType.Compendium);
                GameEventManager.onGuideCompediumUpdateEvent.Invoke();
            }

            hasInteracted = false;
            WorldItemManager.instance.RemoveItemFromWorldItemDictionary(playerGuide.Name, 1);
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