using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPeopleTravellerSaveSystem : MonoBehaviour, ISaveable
{

    public RandomAccessories accessories;
    public RandomColor colors;
    public InteractableBallPeopleTraveller travellerInteractable;
    public BallPeopleTravelerAI travellerAI;
    public QI_ItemDatabase itemDatabase;
    public object CaptureState()
    {

        string q = "";
        if (travellerInteractable.undertaking.undertaking != null)
            q = travellerInteractable.undertaking.undertaking.Name;
        string t = "";
        if (travellerInteractable.undertaking.task != null)
            t = travellerInteractable.undertaking.task.Name;
        return new SaveData
        {
            accessoryIndex = accessories.accessoryIndex,
            r = colors.randomColor.r,
            g = colors.randomColor.g,
            b = colors.randomColor.b,

            travellerDestination = travellerAI.travellerDestination,
            undertakingName = q,
            taskName = t,
            started = travellerInteractable.started,
            hasInteracted = travellerAI.hasInteracted
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        colors.SetColor(saveData.r, saveData.g, saveData.b);
        accessories.PopulateList();
        accessories.SetAccessories(saveData.accessoryIndex);
        if (saveData.undertakingName != "")
            travellerInteractable.undertaking.undertaking = UndertakingDatabaseHolder.instance.undertakingDatabase.GetUndertaking(saveData.undertakingName);
        
        if (saveData.taskName != "")
            travellerInteractable.undertaking.task = UndertakingDatabaseHolder.instance.undertakingDatabase.GetTask(saveData.undertakingName, saveData.taskName);
        travellerInteractable.started = saveData.started;
        travellerAI.hasInteracted = saveData.hasInteracted;
        travellerAI.travellerDestination = saveData.travellerDestination;
    }

    [Serializable]
    private struct SaveData
    {

        public int accessoryIndex;
        public float r;
        public float g;
        public float b;

        public SVector3 travellerDestination;

        public string undertakingName;
        public string taskName;
        public bool started;

        public bool hasInteracted;
    }
}

