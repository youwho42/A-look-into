using Klaxon.SAP;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class SAP_WorldBeliefSaveSystem : MonoBehaviour, ISaveable
    {
        public SAP_WorldBeliefStates world;
        public object CaptureState()
        {
            List<string> _states = new List<string>();
            List<bool> _values = new List<bool>();
            foreach (var state in world.worldStates)
            {
                _states.Add(state.Key);
                _values.Add(state.Value);
            }
            return new SaveData
            {
                states = _states,
                values = _values
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            for (int i = 0; i < saveData.states.Count; i++)
            {
                world.SetWorldState(saveData.states[i], saveData.values[i]);
            }

        }

        [Serializable]
        private struct SaveData
        {
            public List<string> states;
            public List<bool> values;

        }
    } 
}