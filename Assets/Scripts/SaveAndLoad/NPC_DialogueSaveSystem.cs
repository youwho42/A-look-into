using UnityEngine;
using System.Collections.Generic;
using Klaxon.ConversationSystem;
using System;


namespace Klaxon.SaveSystem
{
	public class NPC_DialogueSaveSystem : MonoBehaviour, ISaveable
    {
        public NPC_DialogueSystem dialogueSystem;

        public object CaptureState()
        {
            
            List<bool> parsed = new List<bool>();
            foreach (var d in dialogueSystem.dialogues)
            {
                parsed.Add(d.hasBeenParsed);
            }
            return new SaveData
            {
                dialogueParsed = parsed,
                hasRandomTalked = dialogueSystem.hasRandomTalked
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            for (int i = 0; i < saveData.dialogueParsed.Count; i++)
            {
                if(i < dialogueSystem.dialogues.Count)
                    dialogueSystem.dialogues[i].hasBeenParsed = saveData.dialogueParsed[i];
            }
            dialogueSystem.hasRandomTalked = saveData.hasRandomTalked;
        }


        [Serializable]
        private struct SaveData
        {
            public List<bool> dialogueParsed;
            public bool hasRandomTalked;
        }
    }

}