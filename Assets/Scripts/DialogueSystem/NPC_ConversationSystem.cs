using Klaxon.GOAD;
using Klaxon.UndertakingSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using Klaxon.StatSystem;


namespace Klaxon.ConversationSystem
{
    
    public class NPC_ConversationSystem : MonoBehaviour
    {

        public LocalizedString NPC_Name;
        public StatChanger gumptionCost;
        public List<ConversationObject> conversations = new List<ConversationObject>();
        List<ConversationObject> B_Conversations = new List<ConversationObject>();
        List<ConversationObject> U_Conversations = new List<ConversationObject>();
        GOAD_Scheduler_NPC scheduler_NPC;

        private void Start()
        {
            scheduler_NPC = GetComponent<GOAD_Scheduler_NPC>();

            foreach (var convo in conversations)
            {
                switch (convo.type)
                {
                    case ConversationType.Basic:
                        B_Conversations.Add(convo);
                        break;
                    case ConversationType.Quest:
                        U_Conversations.Add(convo);
                        break;
                }
            }
        }
        public DialogueBranch GetConversation()
        {
            DialogueBranch dialogue = null;
            foreach (var convo in U_Conversations)
            {
                if (convo.Completed)
                    continue;

                if(convo.DialogueCondition != null)
                {
                    if (!scheduler_NPC.IsConditionMet(convo.DialogueCondition))
                        continue;
                }

                if (convo.ActivateUndertakingAtStart)
                    ActivateUndertaking(convo.ActivateUndertakingObject);

                var currentUndertaking = convo.ActivateUndertakingObject;
                
                switch (currentUndertaking.CurrentState)
                {
                    case UndertakingState.Inactive:
                        if (convo.ActivateUndertakingObject)
                            ActivateUndertaking(convo.ActivateUndertakingObject);
                        dialogue = convo.DialogueBranches.FirstOrDefault(o => o.DialogueType == DialogueType.U_Inactive);
                        
                        if (dialogue.UndertakingTask)
                            convo.ActivateUndertakingObject.TryCompleteTask(dialogue.UndertakingTask); 
                        
                        break;

                    case UndertakingState.Active:

                        dialogue = convo.DialogueBranches.FirstOrDefault(o => o.DialogueType == DialogueType.U_Active);
                        if (dialogue.UndertakingTask)
                        {
                            if (!dialogue.UndertakingTask.IsComplete)
                                convo.ActivateUndertakingObject.TryCompleteTask(dialogue.UndertakingTask); 
                        }
                        bool hasCompleteDialogue = false;
                        foreach (var d in convo.DialogueBranches)
                        {
                            if (d.DialogueType == DialogueType.U_Complete)
                                hasCompleteDialogue = true;
                        }
                        convo.Completed = !hasCompleteDialogue;
                        break;

                    case UndertakingState.Complete:
                        dialogue = convo.DialogueBranches.FirstOrDefault(o => o.DialogueType == DialogueType.U_Complete);
                        convo.Completed = true;
                        break;
                    
                }
                if(gumptionCost != null)
                    PlayerInformation.instance.statHandler.ChangeStat(gumptionCost);

                if (dialogue != null)
                    break;
            }

            if(dialogue == null)
            {
                if (B_Conversations.Count > 0)
                {
                    int conversationIndex = UnityEngine.Random.Range(0, B_Conversations.Count);
                    dialogue = B_Conversations[conversationIndex].DialogueBranches[0];
                }
            }
            
            return dialogue;
            
        }

        void ActivateUndertaking(UndertakingObject undertaking)
        {
            undertaking.ActivateUndertaking();
        }

        //bool ConditionalDialogueCheck(ConversationObject conversation)
        //{

        //    if(conversation.DialogueCondition.Condition == "" || !conversation.hasCondition)
        //        return true;


        //    Dictionary<string, bool> temp = new Dictionary<string, bool>(scheduler_NPC.beliefs);
        //    // Combine the two dictionaries without modifying either original dictionary
        //    foreach (var kvp in GOAD_WorldBeliefStates.instance.worldStates)
        //    {
        //        temp[kvp.Key] = kvp.Value;
        //    }
            
        //    bool state;
        //    if (temp.TryGetValue(conversation.DialogueCondition.Condition, out state))
        //    {
        //        if (conversation.DialogueCondition.State == state)
        //            return true;
        //    }
            
        //    return false;
        //}

    }

}
