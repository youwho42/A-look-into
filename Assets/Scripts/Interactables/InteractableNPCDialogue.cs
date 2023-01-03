using QuantumTek.QuantumDialogue;
using QuantumTek.QuantumDialogue.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPCDialogue : Interactable
{

    public QD_DialogueHandler handler;
    QD_DialogueDemo canvasDialogueDisplay;

    private QuestsNPCHolder quests; 
    public QuestsNPCHolder Quests 
    { 
        get 
        { 
            if(quests == null)
                quests= GetComponent<QuestsNPCHolder>();
        return quests; 
        }
    }

   


    public override void Start()
    {
        base.Start();
        canvasDialogueDisplay = FindObjectOfType<QD_DialogueDemo>();
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
        if (Quests != null)
        {
            // get all the quests in questHolder
            // check if any of these quests are active 
            // somewhere we need to know if that quest is completed or not even active
            // compare these quests to the conversation possibilities
            // set the conversation
            // check if quest can be completed
            
            int complete = 0;
            
            for (int i = 0; i < Quests.quests.Length; i++)
            {

                var q = PlayerInformation.instance.playerQuestHandler.GetQuest(Quests.quests[i].Quest.Name);
                if (q.Completed)
                {
                    complete++;
                    continue;
                }

                if (q.Status == QuantumTek.QuantumQuest.QQ_QuestStatus.Active)
                {
                    if(Quests.TurnInQuest(q))
                        canvasDialogueDisplay.handler.SetConversation($"QuestComplete_{q.Name}");
                    else
                        canvasDialogueDisplay.handler.SetConversation($"QuestActive_{q.Name}");
                    
                    break;
                }
                if (q.Status == QuantumTek.QuantumQuest.QQ_QuestStatus.Inactive)
                {
                    PlayerInformation.instance.playerQuestHandler.ActivateQuest(q.Name);
                    GameEventManager.onUndertakingsUpdateEvent.Invoke();
                    NotificationManager.instance.SetNewNotification($"{q.Name} undertaking started", NotificationManager.NotificationType.Undertaking);
                    canvasDialogueDisplay.handler.SetConversation($"QuestInactive_{q.Name}");
                    break;
                }


            }
            if (complete >= Quests.quests.Length)
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
