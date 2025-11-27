using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using Klaxon.StatSystem;
using Klaxon.GOAD;
using Klaxon.UndertakingSystem;
using System.Collections;

namespace Klaxon.ConversationSystem
{
    public class NPC_DialogueSystem : MonoBehaviour
    {
        public LocalizedString NPC_Name;
        public StatChanger gumptionCost;
        public List<DialogueObject> dialogues = new List<DialogueObject>();
        GOAD_Scheduler schedulerBeliefs;

        private void Start()
        {
            SetScheduler();
        }

        void SetScheduler()
        {
            if(schedulerBeliefs == null)
                schedulerBeliefs = GetComponent<GOAD_Scheduler>();
        }
        //[ContextMenu("Get Dialogue")]
        //public void TestDialogue()
        //{
        //    var d = GetDialogue();
        //    Debug.Log(d.ConversationName);
        //}

        public DialogueObject GetDialogue()
        {

            List<DialogueObject> potential = new List<DialogueObject>();
            SetScheduler();
            // Get all potential dialogues
            foreach (var dialogue in dialogues)
            {
                    if (schedulerBeliefs.AreConditionsMet(dialogue.ConditionsNeeded))
                        potential.Add(dialogue);
            }

            // Filter out all the dialogues that are tied to an undertaking that are completed and already parsed
            for (int i = potential.Count - 1; i >= 0; i--)
            {
                if (potential[i].isTiedToUndertaking)
                {
                    if (potential[i].undertakingState != potential[i].undertaking.CurrentState)
                        potential.Remove(potential[i]);
                    else if(potential[i].undertakingState == UndertakingState.Complete && potential[i].hasBeenParsed)
                        potential.Remove(potential[i]);
                    
                }

            }
            if (potential.Count <= 0)
                return null;

            // if one is tied to an undertaking return the first one.
            foreach (var dialogue in potential)
            {
                if (dialogue.isTiedToUndertaking)
                    return dialogue;
            }

            // if there are any we've never seen before.
            List<DialogueObject> unparsed = new List<DialogueObject>();
            foreach (var dialogue in potential)
            {
                if(!dialogue.hasBeenParsed)
                    unparsed.Add(dialogue);
            }
            if(unparsed.Count > 0)
            {
                int r = Random.Range(0, unparsed.Count);
                return unparsed[r];
            }

            // else return a random one of those left
            int rand = Random.Range(0, potential.Count);
            return potential[rand];
        }

        
    }

}