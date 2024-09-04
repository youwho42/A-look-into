using Klaxon.GOAD;
using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class BallPeopleTravellerSaveSystem : MonoBehaviour, ISaveable
    {

        
        public InteractableBallPeopleTraveller travellerInteractable;
        public GOAD_Scheduler_BP travellerAI;
        public QI_ItemDatabase itemDatabase;
        public object CaptureState()
        {

            string q = "";
            if (travellerInteractable.undertaking.undertaking != null)
                q = travellerInteractable.undertaking.undertaking.Name;
            string t = "";
            if (travellerInteractable.undertaking.task != null)
                t = travellerInteractable.undertaking.task.Name;

            List<string> names = new List<string>();
            List<bool> states = new List<bool>();
            foreach (var b in travellerAI.beliefs)
            {
                names.Add(b.Key);
                states.Add(b.Value);
            }
            return new SaveData
            {
                
                travellerDestination = travellerAI.travellerDestination,
                undertakingName = q,
                taskName = t,
                started = travellerInteractable.started,
                hasInteracted = travellerAI.hasInteracted,
                beliefNames = names,
                beliefStates = states

            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            
            if (saveData.undertakingName != "")
                travellerInteractable.undertaking.undertaking = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetUndertaking(saveData.undertakingName);

            if (saveData.taskName != "")
                travellerInteractable.undertaking.task = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetTask(saveData.undertakingName, saveData.taskName);
            travellerInteractable.started = saveData.started;
            travellerAI.hasInteracted = saveData.hasInteracted;
            travellerAI.travellerDestination = saveData.travellerDestination;

            for (int i = 0; i < saveData.beliefNames.Count; i++)
            {
                travellerAI.SetBeliefState(saveData.beliefNames[i], saveData.beliefStates[i]);
            }
        }

        [Serializable]
        private struct SaveData
        {

            public SVector3 travellerDestination;

            public string undertakingName;
            public string taskName;
            public bool started;

            public bool hasInteracted;

            public List<string> beliefNames;
            public List<bool> beliefStates;
        }
    } 
}

