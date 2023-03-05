using Klaxon.UndertakingSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UndertakingsSaveSystem : MonoBehaviour, ISaveable
{
   
    public PlayerUndertakingHandler undertakingHandler;
    
    public object CaptureState()
    {
        List<string> _undertakingNames = new List<string>();
        List<int> _undertakingStates = new List<int>();


        List<string> _taskUndertakingNames = new List<string>();
        List<string> _taskNames = new List<string>();
        List<bool> _taskStates = new List<bool>();

        foreach (var undertaking in undertakingHandler.activeUndertakings)
        {
            _undertakingNames.Add(undertaking.Name);
            _undertakingStates.Add((int)undertaking.CurrentState);
            for (int i = 0; i < undertaking.Tasks.Count; i++)
            {
                _taskUndertakingNames.Add(undertaking.Name);
                _taskNames.Add(undertaking.Tasks[i].Name);
                _taskStates.Add(undertaking.Tasks[i].IsComplete);
            }
        }

        return new SaveData
        {
            undertakingName = _undertakingNames,
            undertakingStates = _undertakingStates,
            taskUndertakingNames = _taskUndertakingNames,
            taskNames = _taskNames,
            taskStates = _taskStates
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        for (int i = 0; i < saveData.undertakingName.Count; i++)
        {
            var q = UndertakingDatabase.instance.GetUndertaking(saveData.undertakingName[i]);
            undertakingHandler.AddUndertaking(q);
            UndertakingDatabase.instance.SetUndertakingState(q, saveData.undertakingStates[i]);
        }
        for (int j = 0; j < saveData.taskUndertakingNames.Count; j++)
        {
            UndertakingDatabase.instance.SetTaskState(saveData.taskUndertakingNames[j], saveData.taskNames[j], saveData.taskStates[j]);
        }
        GameEventManager.onUndertakingsUpdateEvent.Invoke();
    }

    [Serializable]
    private struct SaveData
    {
        public List<string> undertakingName;
        public List<int> undertakingStates;

        public List<string> taskUndertakingNames;
        public List<string> taskNames;
        public List<bool> taskStates;
    }

    
}
