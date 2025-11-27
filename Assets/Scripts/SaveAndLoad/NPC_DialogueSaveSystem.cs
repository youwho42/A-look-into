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
                dialogueParsed = parsed
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            for (int i = 0; i < saveData.dialogueParsed.Count; i++)
            {
                dialogueSystem.dialogues[i].hasBeenParsed = saveData.dialogueParsed[i];
            }
        }


        [Serializable]
        private struct SaveData
        {
            public List<bool> dialogueParsed;
        }
    }

}