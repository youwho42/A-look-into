using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;
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
        public string interactFailSound = "DefaultFail";
        public string interactVerb = "whaaAA???";
        public LocalizedString localizedInteractVerb;
        public LocalizedString longInteractVerb;
        public LocalizedString localizedItemName;

        public StatChanger bounceCost;
        
        public float agencyReward;
        public float agencyCost;
        public bool canInteract = true;

        protected bool hasInteracted;
        protected PlayerInformation playerInformation;

        public bool hasLongInteract;
        [ConditionalHide("hasLongInteract", true)]
        public bool onlyLongInteract;

        public ReplaceObjectOnItemDrop replaceObjectOnDrop;
        public bool canPlaceOnOther;
        public Transform visualItem;

        
        public InstructionObject instruction;
        public QI_ItemData playerGuide;


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
            if (bounceCost == null)
                return true;
            if (PlayerInformation.instance.statHandler.GetStatCurrentModifiedValue("Bounce") >= Mathf.Abs(bounceCost.Amount))
            {
                
                return true;
            }

            Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Missing bounce"), null, 0, NotificationsType.Warning);

            //NotificationManager.instance.SetNewNotification("You are missing Bounce to do this.", NotificationManager.NotificationType.Warning);
            return false;
        }

        public bool HasReadHowTo()
        {
            if (instruction == null)
                return true;
            //if (SAP_WorldBeliefStates.instance.HasWorldState(instruction.condition.Condition, instruction.condition.State))
            //    return true;
            BallPersonMessageDisplayUI.instance.ShowSimpleMessage(instruction.title.GetLocalizedString(), instruction.description.GetLocalizedString());
            UIScreenManager.instance.DisplayIngameUI(UIScreenType.BallPersonDialogueUI, true);            //SAP_WorldBeliefStates.instance.SetWorldState(instruction.condition.Condition, instruction.condition.State);
            return false;
        }

        public void SetGuideOrNote()
        {
            if (playerGuide == null)
                return;
            if (!PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Contains(playerGuide))
            {
                PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Add(playerGuide);
                Notifications.instance.SetNewNotification($"{playerGuide.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Compendium);
                GameEventManager.onGuideCompediumUpdateEvent.Invoke();
            }
        }
    } 
}
