using Klaxon.SAP;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using SerializableTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BallPeopleSeekerSaveSystem : MonoBehaviour, ISaveable
{
    public RandomAccessories accessories;
    public RandomColor colors;
    public InteractableBallPeopleSeeker seekerInteractable;
    public SAP_Scheduler_BP seekerAI;
    public QI_ItemDatabase itemDatabase;

    public object CaptureState()
    {

        string q = "";
        if (seekerInteractable.talkTask.undertaking != null)
            q = seekerInteractable.talkTask.undertaking.Name;
        string t = "";
        if (seekerInteractable.talkTask.task != null)
            t = seekerInteractable.talkTask.task.Name;
        string s = "";
        if (seekerAI.task.task != null)
            s = seekerAI.task.task.Name;

        List<SVector3> loc = new List<SVector3>();
        foreach (var l in seekerAI.seekItemsFound)
        {
            loc.Add(l);
        }

        return new SaveData
        {
            accessoryIndex = accessories.accessoryIndex,
            r = colors.randomColor.r,
            g = colors.randomColor.g,
            b = colors.randomColor.b,

            undertakingName = q,
            talkTaskName = t,
            seekTaskName = s,
            started = seekerInteractable.started,
            seekLocations = loc,
            seekAmount = seekerAI.seekAmount,
            foundAmount = seekerAI.foundAmount,
            hasInteracted = seekerAI.hasInteracted, 
            seekItem = seekerAI.seekItem.Name
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        colors.SetColor(saveData.r, saveData.g, saveData.b);
        accessories.PopulateList();
        accessories.SetAccessories(saveData.accessoryIndex);
        if (saveData.undertakingName != "")
        {
            seekerInteractable.talkTask.undertaking = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetUndertaking(saveData.undertakingName);
            seekerAI.task.undertaking = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetUndertaking(saveData.undertakingName);
        }

        if (saveData.talkTaskName != "")
            seekerInteractable.talkTask.task = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetTask(saveData.undertakingName, saveData.talkTaskName);

        if (saveData.seekTaskName != "")
            seekerAI.task.task = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetTask(saveData.undertakingName, saveData.seekTaskName);

        foreach (var l in saveData.seekLocations)
        {
            seekerAI.seekItemsFound.Add(l);
        }

        seekerInteractable.started = saveData.started;
        seekerAI.seekAmount = saveData.seekAmount;
        seekerAI.foundAmount = saveData.foundAmount;
        seekerAI.hasInteracted = saveData.hasInteracted;
        seekerAI.seekItem = itemDatabase.GetItem(saveData.seekItem);
    }

    [Serializable]
    private struct SaveData
    {

        public int accessoryIndex;
        public float r;
        public float g;
        public float b;

        

        public string undertakingName;
        public string talkTaskName;
        public string seekTaskName;
        public bool started;

        public int seekAmount;
        public int foundAmount;
        public List<SVector3> seekLocations;
        public bool hasInteracted;
        public string seekItem;
    }
}
