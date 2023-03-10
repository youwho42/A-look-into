using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumDialogue;
using QuantumTek.QuantumDialogue.Demo;
using UnityEngine;

public class ReadInteractable : Interactable
{
    public QD_DialogueHandler dialogueHandler;

    public string coversation;

    public QD_DialogueDemo canvasDialogueDisplay;

    public override void Start()
    {
        base.Start();
        
    }
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        canInteract = false;
        Read();
    }

    private void Read()
    {
        
        UIScreenManager.instance.DisplayScreen(UIScreenType.DialogueDisplayScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        canvasDialogueDisplay.handler = dialogueHandler;
        canvasDialogueDisplay.handler.SetConversation(coversation);
        canvasDialogueDisplay.SetText();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            canInteract = true;
            
        }

    }
}
