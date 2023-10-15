using Klaxon.UndertakingSystem;
using System;
using System.Collections;
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

        private void Start()
        {
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

                if (convo.ActivateUndertakingObject && convo.ActivateAtStart)
                    ActivateUndertaking(convo.ActivateUndertakingObject);

                if (convo.UndertakingTask)
                    convo.UndertakingObject.TryCompleteTask(convo.UndertakingTask);

                var currObject = convo.ActivateUndertakingObject == null ? convo.UndertakingObject : convo.ActivateUndertakingObject;
                switch (currObject.CurrentState)
                {
                    case UndertakingState.Inactive:
                        if (convo.ActivateUndertakingObject)
                            ActivateUndertaking(convo.ActivateUndertakingObject);
                        dialogue = convo.DialogueBranches.FirstOrDefault(o => o.DialogueType == DialogueType.U_Inactive);
                        break;

                    case UndertakingState.Active:
                        dialogue = convo.DialogueBranches.FirstOrDefault(o => o.DialogueType == DialogueType.U_Active);
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

    }

}
