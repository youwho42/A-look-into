using Klaxon.ConversationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDialogue : Interactable
{

    NPC_ConversationSystem dialogueSystem;
    DialogueBranch currentDialogue;

    public override void Start()
    {
        base.Start();
        dialogueSystem = GetComponent<NPC_ConversationSystem>();
        interactVerb = $"Talk to {dialogueSystem.NPC_Name}";
    }
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
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
