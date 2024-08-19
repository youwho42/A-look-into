using Klaxon.UndertakingSystem;
using System;
using Klaxon.GOAD;
using UnityEngine;
using UnityEngine.Localization;

namespace Klaxon.ConversationSystem
{
    [Serializable]
    public class DialogueBranch
    {
        public DialogueType DialogueType;

        //public UndertakingObject UndertakingObject;
        public UndertakingTaskObject UndertakingTask;

        public LocalizedString[] localizedSentences;
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

        public bool hasCondition;
        [ConditionalHide("hasCondition", true)]
        public GOAD_ScriptableCondition DialogueCondition;

        public bool ActivateUndertakingAtStart;
        public UndertakingObject ActivateUndertakingObject;

        //public UndertakingObject UndertakingObject;
        //public UndertakingTaskObject UndertakingTask;

        public DialogueBranch[] DialogueBranches;
        //[HideInInspector]
        public bool Completed;



        public void ResetConversation()
        {
            Completed = false;
        }
    }
}

