using Klaxon.UndertakingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.ConversationSystem
{
    [Serializable]
    public class DialogueBranch
    {
        public DialogueType DialogueType;
        [TextArea(3, 15)]
        public string[] sentences;
        
    }
    public enum ConversationType
    {
        Basic,
        Quest
    }


    public enum DialogueType
    {
        None,
        U_Inactive,
        U_Active,
        U_Complete
    }
    [CreateAssetMenu(menuName = "Klaxon/Dialogue Obect", fileName = "New_Dialogue_Object")]
    public class ConversationObject : ScriptableObject
    {
        
        public string DialogueName;
        public ConversationType type;

        public bool ActivateAtStart;
        public UndertakingObject ActivateUndertakingObject;

        public UndertakingObject UndertakingObject;
        public UndertakingTaskObject UndertakingTask;

        public DialogueBranch[] DialogueBranches;
        [HideInInspector]
        public bool Completed;

        public void ResetConversation()
        {
            Completed = false;
        }
    }
}

