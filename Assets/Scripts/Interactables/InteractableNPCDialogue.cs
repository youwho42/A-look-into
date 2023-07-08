using Klaxon.UndertakingSystem;
using QuantumTek.QuantumDialogue;
using QuantumTek.QuantumDialogue.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPCDialogue : Interactable
{
    public string NPC_name;
    public QD_DialogueHandler handler;
    QD_DialogueDemo canvasDialogueDisplay;

    private UndertakingHolder undertakings;
    public UndertakingHolder Undertakings
    {
        get
        {
            if (undertakings == null)
                undertakings = GetComponent<UndertakingHolder>();
            return undertakings;
        }
    }
    
    private CompleteTaskOnInteraction talkQuest;
    public CompleteTaskOnInteraction TalkQuest
    {
        get
        {
            if (talkQuest == null)
                talkQuest = GetComponent<CompleteTaskOnInteraction>();
            return talkQuest;
        }
    }




    public override void Start()
    {
        base.Start();
        canvasDialogueDisplay = FindObjectOfType<QD_DialogueDemo>();
        interactVerb = $"Talk to {NPC_name}";
    }
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        canInteract = false;
        Talk();
    }

    
    private void Talk()
    {
        List<string> conversations = AllConversations();
        int conversationIndex = Random.Range(0, conversations.Count);
        UIScreenManager.instance.DisplayScreen(UIScreenType.DialogueDisplayScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        canvasDialogueDisplay.handler = handler;
        if (TalkQuest != null)
            TalkQuest.task.undertaking.ActivateUndertaking();


        if (Undertakings != null)
        {
            // get all the active quests
            // somewhere we need to know if that quest is completed
            // compare these quests to the conversation possibilities
            // set the conversation
            // check if quest can be completed

            int complete = 0;

            for (int i = 0; i < Undertakings.undertakings.Count; i++)
            {
                var u = Undertakings.undertakings[i];
                if (u.CurrentState == UndertakingState.Complete)
                {
                    complete++;
                    continue;
                }

                if (u.CurrentState == UndertakingState.Active)
                {
                    canvasDialogueDisplay.handler.SetConversation($"QuestActive_{u.Name}");
                    break;
                }
                if (u.CurrentState == UndertakingState.Inactive)
                {
                    
                    u.ActivateUndertaking();
                    //GameEventManager.onUndertakingsUpdateEvent.Invoke();
                    
                    canvasDialogueDisplay.handler.SetConversation($"QuestInactive_{u.Name}");
                    break;
                }


            }
            if (complete >= Undertakings.undertakings.Count)
            {
                List<int> nonQuestConversations = NonQuestConversations(conversations);
                conversationIndex = nonQuestConversations[Random.Range(0, nonQuestConversations.Count)];
                canvasDialogueDisplay.handler.SetConversation(handler.dialogue.Conversations[conversationIndex].Name);
            }
        }
        else
        {
            canvasDialogueDisplay.handler.SetConversation(handler.dialogue.Conversations[conversationIndex].Name);
        }
        if (TalkQuest != null)
            TalkQuest.CompleteTask();

        canvasDialogueDisplay.SetInteractableNPC(this);
        canvasDialogueDisplay.SetText();
    }

    List<string> AllConversations()
    {
        List<QD_Conversation> all = handler.dialogue.Conversations;
        List<string> conversations = new List<string>();
        for (int i = 0; i < all.Count; i++)
        {
            conversations.Add(all[i].Name);
        }
        return conversations;
    }
    List<int> NonQuestConversations(List<string> allConversations)
    {
        List<int> conversations = new List<int>();
        for (int i = 0; i < allConversations.Count; i++)
        {
            if (!allConversations[i].Contains("Quest"))
                conversations.Add(i);
        }

        
        return conversations;
    }

}
