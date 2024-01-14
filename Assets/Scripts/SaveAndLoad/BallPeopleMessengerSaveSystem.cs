using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;


namespace Klaxon.SaveSystem
{
    public class BallPeopleMessengerSaveSystem : MonoBehaviour, ISaveable
    {

        
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
                
                messageItem = m,
                messageType = (int)messenger.type,
                undertakingName = u
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            
            if (saveData.messageItem != "")
                messenger.messageItem = itemDatabase.GetItem(saveData.messageItem);
            messenger.type = (BallPeopleMessageType)saveData.messageType;
            if (saveData.undertakingName != "")
            {
                var q = Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.GetUndertaking(saveData.undertakingName);
                messenger.undertaking = q;
            }

        }

        [Serializable]
        private struct SaveData
        {

           

            public string messageItem;
            public int messageType;

            public string undertakingName;
        }
    }

}