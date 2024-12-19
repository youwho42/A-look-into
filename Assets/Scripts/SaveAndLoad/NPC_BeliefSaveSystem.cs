using Klaxon.GOAD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class NPC_BeliefSaveSystem : MonoBehaviour, ISaveable
    {
        public GOAD_Scheduler scheduler;

        public object CaptureState()
        {
            List<string> allBeliefs = new List<string>();
            List<bool> allStates = new List<bool>();
            foreach (var b in scheduler.beliefs)
            {
                allBeliefs.Add(b.Key);
                allStates.Add(b.Value);
            }
            return new SaveData
            {
                beliefs = allBeliefs,
                states = allStates
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            for (int i = 0; i < saveData.beliefs.Count; i++)
            {
                scheduler.SetBeliefState(saveData.beliefs[i], saveData.states[i]);
            }
        }


        [Serializable]
        private struct SaveData
        {
            public List<string> beliefs;
            public List<bool> states;
        }
    }
}
