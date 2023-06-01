using Klaxon.GOAP;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_WorldStateSaveSystem : MonoBehaviour, ISaveable
{
    public GOAP_World world;
    public object CaptureState()
    {
        List<string> _states = new List<string>();
        List<int> _values = new List<int>();
        foreach (var state in world.GetWorld().GetStates())
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
            world.GetWorld().AddState(saveData.states[i], saveData.values[i]);
        }

    }

    [Serializable]
    private struct SaveData
    {
        public List<string> states;
        public List<int> values;

    }
}