using Klaxon.ConversationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.Interactable
{
    public class InteractableDialogue : Interactable
    {

        NPC_ConversationSystem dialogueSystem;
        DialogueBranch currentDialogue;

        public override void Start()
        {
            base.Start();
            dialogueSystem = GetComponent<NPC_ConversationSystem>();
        }
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (GumptionCost())
            {
                currentDialogue = dialogueSystem.GetConversation();
                if (currentDialogue != null)
                {
                    canInteract = false;
                    DialogueManagerUI.instance.SetNewDialogue(this, dialogueSystem, currentDialogue);
                    UIScreenManager.instance.DisplayScreen(UIScreenType.DialogueDisplayScreen);
                    UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
                    PlayerInformation.instance.uiScreenVisible = true;
                    PlayerInformation.instance.TogglePlayerInput(false);
                } 
            }

        }

        bool GumptionCost()
        {

            if (gumptionCost == null)
                return true;


            float gumption = PlayerInformation.instance.statHandler.GetStatCurrentModifiedValue("Gumption");
            if (gumption >= Mathf.Abs(gumptionCost.Amount))
            {
                PlayerInformation.instance.statHandler.ChangeStat(gumptionCost);
                return true;
            }
            Notifications.instance.SetNewNotification($"{Mathf.Abs(gumptionCost.Amount - gumption)} <sprite name=\"Gumption\">", null, 0, NotificationsType.Warning);
            return false;
        }

    } 
}

