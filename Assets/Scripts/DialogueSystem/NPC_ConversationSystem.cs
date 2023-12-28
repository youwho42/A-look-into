using Klaxon.SAP;
using Klaxon.UndertakingSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

namespace Klaxon.ConversationSystem
{
    
    public class NPC_ConversationSystem : MonoBehaviour
    {

        public LocalizedString NPC_Name;
        
        public List<ConversationObject> conversations = new List<ConversationObject>();
        List<ConversationObject> B_Conversations = new List<ConversationObject>();
        List<ConversationObject> U_Conversations = new List<ConversationObject>();
        SAP_Scheduler_NPC scheduler_NPC;

        private void Start()
        {
            scheduler_NPC = GetComponent<SAP_Scheduler_NPC>();

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
                if (convo.Completed || !ConditionalDialogueCheck(convo))
                    continue;

                if (convo.ActivateUndertakingObject && convo.ActivateAtStart)
                    ActivateUndertaking(convo.ActivateUndertakingObject);

                //if (convo.UndertakingTask)
                //    convo.UndertakingObject.TryCompleteTask(convo.UndertakingTask);

                var currObject = convo.ActivateUndertakingObject == null ? convo.UndertakingObject : convo.ActivateUndertakingObject;
                switch (currObject.CurrentState)
                {
                    case UndertakingState.Inactive:
                        if (convo.ActivateUndertakingObject)
                            ActivateUndertaking(convo.ActivateUndertakingObject);
                        dialogue = convo.DialogueBranches.FirstOrDefault(o => o.DialogueType == DialogueType.U_Inactive);
                        if (convo.UndertakingTask)
                            convo.UndertakingObject.TryCompleteTask(convo.UndertakingTask);
                        break;

                    case UndertakingState.Active:
                        if (!convo.UndertakingTask.IsComplete)
                        {
                            dialogue = convo.DialogueBranches.FirstOrDefault(o => o.DialogueType == DialogueType.U_Active);
                            if (convo.UndertakingTask)
                                convo.UndertakingObject.TryCompleteTask(convo.UndertakingTask);
                        }
                            
                        
                        break;

                    case UndertakingState.Complete:
                        dialogue = convo.DialogueBranches.FirstOrDefault(o => o.DialogueType == DialogueType.U_Complete);
                        convo.Completed = true;
                        break;
                    
                }
                
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

        bool ConditionalDialogueCheck(ConversationObject conversation)
        {

            if(conversation.DialogueCondition.Condition == "" || !conversation.hasCondition)
                return true;


            Dictionary<string, bool> temp = new Dictionary<string, bool>(scheduler_NPC.beliefs);
            // Combine the two dictionaries without modifying either original dictionary
            foreach (var kvp in SAP_WorldBeliefStates.instance.worldStates)
            {
                temp[kvp.Key] = kvp.Value;
            }
            
            bool state;
            if (temp.TryGetValue(conversation.DialogueCondition.Condition, out state))
            {
                if (conversation.DialogueCondition.State == state)
                    return true;
            }
            
            return false;
        }

    }

}
