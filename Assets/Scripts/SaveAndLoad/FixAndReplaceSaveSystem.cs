using Klaxon.UndertakingSystem;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAndReplaceSaveSystem : MonoBehaviour, ISaveable
{

    public FixAndReplace fixAndReplace;
    public object CaptureState()
    {
        return new SaveData
        {
            undertakingName = fixAndReplace.undertakingObject.undertaking.Name,
            taskName = fixAndReplace.undertakingObject.task.Name
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        
        if (saveData.undertakingName != "")
            fixAndReplace.undertakingObject.undertaking = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetUndertaking(saveData.undertakingName);

        if (saveData.taskName != "")
            fixAndReplace.undertakingObject.task = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetTask(saveData.undertakingName, saveData.taskName);

    }

    [Serializable]
    private struct SaveData
    {
        public string undertakingName;
        public string taskName;
    }
}
