using Klaxon.GOAD;
using Klaxon.Interactable;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class BallPeopleVillagerSaveSystem : MonoBehaviour, ISaveable
    {


        public InteractableBallPeopleVillager villagerInteractable;
        public GOAD_Scheduler_BP villagerAI;
        
        public object CaptureState()
        {

            string q = "";
            if (villagerInteractable.undertaking.undertaking != null)
                q = villagerInteractable.undertaking.undertaking.Name;
            string t = "";
            if (villagerInteractable.undertaking.task != null)
                t = villagerInteractable.undertaking.task.Name;

            List<string> names = new List<string>();
            List<bool> states = new List<bool>();
            foreach (var b in villagerAI.beliefs)
            {
                names.Add(b.Key);
                states.Add(b.Value);
            }
            return new SaveData
            {

                undertakingName = q,
                taskName = t,
                started = villagerInteractable.started,
                hasInteracted = villagerAI.hasInteracted,
                beliefNames = names,
                beliefStates = states

            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;

            if (saveData.undertakingName != "")
                villagerInteractable.undertaking.undertaking = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetUndertaking(saveData.undertakingName);

            if (saveData.taskName != "")
                villagerInteractable.undertaking.task = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetTask(saveData.undertakingName, saveData.taskName);
            villagerInteractable.started = saveData.started;
            villagerAI.hasInteracted = saveData.hasInteracted;
            

            for (int i = 0; i < saveData.beliefNames.Count; i++)
            {
                villagerAI.SetBeliefState(saveData.beliefNames[i], saveData.beliefStates[i]);
            }
        }

        [Serializable]
        private struct SaveData
        {

            

            public string undertakingName;
            public string taskName;
            public bool started;

            public bool hasInteracted;

            public List<string> beliefNames;
            public List<bool> beliefStates;
        }
    }
}

