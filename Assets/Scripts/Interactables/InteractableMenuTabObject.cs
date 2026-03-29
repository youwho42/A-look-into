using UnityEngine;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using System.Collections;
using Klaxon.GOAD;
using System;
using UnityEngine.Localization;
using Klaxon.UndertakingSystem;

namespace Klaxon.Interactable 
{ 
    public class InteractableMenuTabObject : Interactable
    {
        
        [Serializable]
        public struct MenuObject
        {
            public NotificationsType notificationsType;
            public GOAD_ScriptableCondition typeCondition;
            public LocalizedString typeText;
        }

        public MenuObject menuObject;
        
        QI_Item interactableItem;
        public UndertakingObject undertakingToActivate;

        public override void Start()
        {
            base.Start();
           interactableItem = GetComponent<QI_Item>();
        }
        public override void SetInteractVerb()
        {
            interactVerb = localizedInteractVerb.GetLocalizedString();
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

            if(undertakingToActivate != null)
                playerInformation.playerUndertakings.AddUndertaking(undertakingToActivate);

            PlayInteractSound(true);
            visualItem.gameObject.SetActive(false);
            AudioManager.instance.PlaySound("Discovery");
            
            Notifications.instance.SetNewNotification(menuObject.typeText.GetLocalizedString(), null, 0, menuObject.notificationsType);
            
            yield return new WaitForSeconds(0.5f);

            //GameEventManager.onMapDisplayEvent.Invoke();
            InvokeGameEvent();

            GOAD_WorldBeliefStates.instance.SetWorldState(menuObject.typeCondition);
            
            WorldItemManager.instance.RemoveItemFromWorldItemDictionary(interactableItem.Data.Name, 1);
            Destroy(gameObject);
        }

        void InvokeGameEvent()
        {
            switch (menuObject.notificationsType)
            {
                case NotificationsType.Compendium:
                    GameEventManager.onCompendiumDisplayEvent.Invoke();
                    break;
                case NotificationsType.Inventory:
                    break;
                case NotificationsType.Agency:
                    break;
                case NotificationsType.UndertakingStart:
                    break;
                case NotificationsType.UndertakingComplete:
                    break;
                case NotificationsType.Warning:
                    break;
                case NotificationsType.Map:
                    GameEventManager.onMapDisplayEvent.Invoke();
                    break;
                case NotificationsType.None:
                    break;
                
            }
        }


        void PlayInteractSound(bool success)
        {
            string sound = success ? interactSound : interactFailSound;
            if (audioManager.CompareSoundNames("PickUp-" + sound))
            {
                audioManager.PlaySound("PickUp-" + sound);
            }
        }


    }
}
