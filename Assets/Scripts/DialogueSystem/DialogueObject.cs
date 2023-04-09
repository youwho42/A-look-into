using Klaxon.UndertakingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.DialogueSystem
{
    [Serializable]
    public class DialogueBranch
    {
        public string BranchName;
        [TextArea(3, 15)]
        public string[] sentences;
    }
    [CreateAssetMenu(menuName = "Dialogues/Dialogue")]
    public class DialogueObject : ScriptableObject
    {
        public string SpeakerName;
        public string DialogueName;
        public UndertakingObject Undertaking;


        public DialogueBranch[] DialogueBranches;
    }
}

