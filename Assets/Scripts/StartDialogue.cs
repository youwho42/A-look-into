using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumDialogue;
using QuantumTek.QuantumDialogue.Demo;
using UnityEngine;

public class StartDialogue : Interactable
{
    public QD_DialogueHandler dialogueHandler;

    public string firstConversation;
    bool hadFirstConversation;
    bool hasQuest;
    public string questConversation;
    public string questUpdateConversation;
    public string questCompleteConversation;
    public string endConversation;
    public QD_DialogueDemo canvasDialogueDisplay;
    StartGatherQuest gatherQuest;
    
    Animator animator;

    public override void Start()
    {
        base.Start();
        TryGetComponent<Animator>(out animator);
        gatherQuest = GetComponent<StartGatherQuest>();
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        canInteract = false;
        StartConversation();
    }
    private void StartConversation()
    {
        animator?.SetBool("IsTalking", true);
        canvasDialogueDisplay.DisplayDialoguePanel();
        canvasDialogueDisplay.handler = dialogueHandler;
        if (!hadFirstConversation)
        {
            canvasDialogueDisplay.handler.SetConversation(firstConversation);
            hadFirstConversation = true;
        }
        else if(hadFirstConversation && !hasQuest)
        {
            canvasDialogueDisplay.handler.SetConversation(questConversation);
            canvasDialogueDisplay.playerMadeChoice.AddListener(AcceptQuest);
        }
        else if(hadFirstConversation && hasQuest && !gatherQuest.IsQuestComplete())
        {
            canvasDialogueDisplay.handler.SetConversation(questUpdateConversation);
            canvasDialogueDisplay.playerMadeChoice.AddListener(TurnInQuest);
        }
        else
        {
            canvasDialogueDisplay.handler.SetConversation(endConversation);
        }


        canvasDialogueDisplay.SetText();
        
    }

    void CompleteQuest()
    {
        Debug.Log("Quest Complete!");
        
    }

    void TurnInQuest()
    {

        
        gatherQuest.TurnInQuest();
        canvasDialogueDisplay.playerMadeChoice.RemoveListener(TurnInQuest);
        if (gatherQuest.IsQuestComplete())
        {
            
            
            canvasDialogueDisplay.handler.SetConversation(questCompleteConversation);
            canvasDialogueDisplay.SetText();
            CompleteQuest();
        }
        
        

    }

    void AcceptQuest()
    {
        gatherQuest.StartQuest();
        hasQuest = true;
        canvasDialogueDisplay.playerMadeChoice.RemoveListener(AcceptQuest);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            
            animator?.SetBool("IsTalking", false);
            canvasDialogueDisplay.HideDialoguePanel();
            canInteract = true;
        }

    }


}
