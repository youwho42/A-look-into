using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPeopleMessengerSaveSystem : MonoBehaviour, ISaveable
{

    public RandomAccessories accessories;
    public RandomColor colors;
    public InteractableBallPeopleMessenger messenger;
    public QI_ItemDatabase itemDatabase;
    public object CaptureState()
    {

        string m = "";
        if (messenger.messageItem != null)
            m = messenger.messageItem.Name;
        string u = "";
        if (messenger.undertaking != null)
            u = messenger.undertaking.Name;
        return new SaveData
        {
            accessoryIndex = accessories.accessoryIndex,
            r = colors.randomColor.r,
            g = colors.randomColor.g,
            b = colors.randomColor.b,
            messageItem = m,
            messageType = (int)messenger.type,
            undertakingName = u
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        colors.SetColor(saveData.r, saveData.g, saveData.b);
        accessories.PopulateList();
        accessories.SetAccessories(saveData.accessoryIndex);
        if(saveData.messageItem != "")
            messenger.messageItem = itemDatabase.GetItem(saveData.messageItem);
        messenger.type = (BallPeopleMessageType)saveData.messageType;
        if(saveData.undertakingName != "") 
        {
            var q = UndertakingDatabaseHolder.instance.undertakingDatabase.GetUndertaking(saveData.undertakingName);
            messenger.undertaking = q;
        }
        
    }

    [Serializable]
    private struct SaveData
    {
        
        public int accessoryIndex;
        public float r;
        public float g;
        public float b;

        public string messageItem;
        public int messageType;

        public string undertakingName;
    }
}
