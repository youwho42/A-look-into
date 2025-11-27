using UnityEngine;
using System;
using Klaxon.UndertakingSystem;
using UnityEngine.Localization;
using Klaxon.GOAD;
using QuantumTek.QuantumInventory;
using System.Collections.Generic;

namespace Klaxon.ConversationSystem
{


    [Serializable]
    public class DialogueNode
    {
        [HideInInspector]
        public int NodeIndex;

        public LocalizedString LocalizedPhrase;

        public bool HasChoices;
        public List<Choice> Choices = new List<Choice>();
        public int AutoNextNodeID = -1;


        public bool SetsCondition;
        [ConditionalHide("SetsCondition", true)]
        public GOAD_ScriptableCondition Condition;

        public bool SetsUndertaking;
        [ConditionalHide("SetsUndertaking", true)]
        public UndertakingObject Undertaking;

        public bool CompleteTask;
        [ConditionalHide("CompleteTask", true)]
        public UndertakingTaskObject Task;

        public bool GivesItem;
        [ConditionalHide("GivesItem", true)]
        public QI_ItemData Item;
        [ConditionalHide("GivesItem", true)]
        public int Amount;

        
    }
    [Serializable]
    public class Choice
    {
        public LocalizedString LocalizedChoice;
        public int NextNodeID = -1;

        public bool HasConditions;
        public List<GOAD_ScriptableCondition> ConditionsNeeded = new List<GOAD_ScriptableCondition>();
    }

    [CreateAssetMenu(menuName = "Klaxon/New Dialogue", fileName = "New_Dialogue")]
    public class DialogueObject : ScriptableObject
    {

        public string ConversationName;

        public List<GOAD_ScriptableCondition> ConditionsNeeded = new List<GOAD_ScriptableCondition>();

        public bool isTiedToUndertaking;
        [ConditionalHide("isTiedToUndertaking", true)]
        public UndertakingObject undertaking;
        [ConditionalHide("isTiedToUndertaking", true)]
        public UndertakingState undertakingState;
        [HideInInspector]
        public int startPhraseID = 0;

        public List<DialogueNode> dialogueNodes = new List<DialogueNode>();

        public bool hasBeenParsed;



        public void ResetConversation()
        {
            hasBeenParsed = false;
        }




    }
}
