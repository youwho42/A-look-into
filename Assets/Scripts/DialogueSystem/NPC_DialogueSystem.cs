using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using Klaxon.StatSystem;
using Klaxon.GOAD;
using Klaxon.UndertakingSystem;
using System.Collections;

namespace Klaxon.ConversationSystem
{
    public class NPC_DialogueSystem : MonoBehaviour, IResetAtDawn
    {
        public LocalizedString NPC_Name;
        public StatChanger gumptionCost;
        public List<DialogueObject> dialogues = new List<DialogueObject>();
        GOAD_Scheduler schedulerBeliefs;
        public bool hasRandomTalked;
        [Range(1.2f, 2.0f)]
        public float voicePitch = 1.6f;


        private void Start()
        {
            SetScheduler();
            
        }

        void SetScheduler()
        {
            if(schedulerBeliefs == null)
                schedulerBeliefs = GetComponent<GOAD_Scheduler>();
        }
        

        public DialogueObject GetDialogue()
        {

            // Get all potential dialogues
            List<DialogueObject> potential = GetPotentialDialogues();


            // Filter out all the dialogues that are tied to an undertaking that are completed and already parsed
            List<DialogueObject> undertakings = GetUndertakingsList(potential);
            // if one is tied to an undertaking return the first one.
            if (undertakings.Count > 0) 
                return undertakings[0];
            

            
            if(hasRandomTalked)
                return null;
            // Get a random dialogue
            DialogueObject random = GetRandomDialogue(potential);
            if(random == null)
                return null;
            hasRandomTalked = true;
            return random;
            
        }

        public DialogueObject CurrentOpenUndertaking()
        {
            // Get all potential dialogues
            List<DialogueObject> potential = GetPotentialDialogues();
            // Filter out all the dialogues that are tied to an undertaking that are completed and already parsed
            List<DialogueObject> undertakings = GetUndertakingsList(potential);
            
            // if one is tied to an undertaking return the first one.
            if (undertakings.Count > 0)
                return undertakings[0];

            return null;
        }

        List<DialogueObject> GetPotentialDialogues()
        {
            SetScheduler();

            List <DialogueObject> potential = new List<DialogueObject>();
            foreach (var dialogue in dialogues)
            {
                if (schedulerBeliefs.AreConditionsMet(dialogue.ConditionsNeeded))
                    potential.Add(dialogue);
            }
            return potential;
        }

        List<DialogueObject> GetUndertakingsList(List<DialogueObject> potential)
        {
            List<DialogueObject> undertakings = new List<DialogueObject>();
            for (int i = potential.Count - 1; i >= 0; i--)
            {
                if (potential[i].isTiedToUndertaking)
                {
                    if (potential[i].undertakingState != potential[i].undertaking.CurrentState)
                        potential.Remove(potential[i]);
                    else if (potential[i].undertakingState == UndertakingState.Complete && potential[i].hasBeenParsed)
                        potential.Remove(potential[i]);
                    else
                        undertakings.Add(potential[i]);
                }

            }
            return undertakings;
        }

        DialogueObject GetRandomDialogue(List<DialogueObject> potential)
        {
            List<DialogueObject> randoms = new List<DialogueObject>();
            List<DialogueObject> unparsed = new List<DialogueObject>();
            for (int i = potential.Count - 1; i >= 0; i--)
            {
                if (!potential[i].isTiedToUndertaking)
                {
                    if (!potential[i].hasBeenParsed)
                        unparsed.Add(potential[i]);
                    randoms.Add(potential[i]);
                }
                    
            }
            
            if (unparsed.Count > 0)
            {
                int r = Random.Range(0, unparsed.Count);
               
                return unparsed[r];
            }
            // else return a random one of those left
            int rand = Random.Range(0, randoms.Count);
            
            return randoms[rand];
        }

        public void ResetAtDawn()
        {
            hasRandomTalked = false;
        }
    }

}