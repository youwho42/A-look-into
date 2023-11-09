using Klaxon.StatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Klaxon.Interactable
{
    public class Interactable : MonoBehaviour
    {
        protected AudioManager audioManager;
        public string interactSound = "Default";
        public string interactVerb = "whaaAA???";
        public LocalizedString localizedInteractVerb;
        public LocalizedString longInteractVerb;

        public StatChanger bounceCost;
        public StatChanger gumptionCost;
        public float agencyReward;
        public float agencyCost;
        public bool canInteract = true;

        protected bool hasInteracted;
        protected PlayerInformation playerInformation;

        public bool hasLongInteract;

        public ReplaceObjectOnItemDrop replaceObjectOnDrop;
        public bool canPlaceOnOther;
        public Transform visualItem;

        public virtual void Start()
        {
            audioManager = AudioManager.instance;
            playerInformation = PlayerInformation.instance;
        }

        public virtual void SetInteractVerb()
        {
            interactVerb = localizedInteractVerb.GetLocalizedString();
        }

        public virtual void Interact(GameObject interactor)
        {
            if (hasInteracted)
                return;



            hasInteracted = true;

            // The rest happens in child script...
        }
        public virtual void LongInteract(GameObject interactor)
        {
            if (hasInteracted || !hasLongInteract)
                return;



            hasInteracted = true;

            // The rest happens in child script...
        }


        public bool InteractBounceCost()
        {
            if (PlayerInformation.instance.statHandler.GetStatCurrentModifiedValue("Bounce") >= Mathf.Abs(bounceCost.Amount))
            {
                PlayerInformation.instance.statHandler.ChangeStat(bounceCost);
                return true;
            }

            Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Missing bounce"), null, 0, NotificationsType.Warning);

            //NotificationManager.instance.SetNewNotification("You are missing Bounce to do this.", NotificationManager.NotificationType.Warning);
            return false;
        }


    } 
}
