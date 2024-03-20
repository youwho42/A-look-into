using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class CraftingStationSaveSystem : MonoBehaviour, ISaveable
    {
        public QI_CraftingHandler craftingHandler;
        public QI_ItemDatabase itemDatabase;


        public object CaptureState()
        {
            List<string> names = new List<string>();
            List<int> amounts = new List<int>();
            List<float> times = new List<float>();
            List<float> maxTimes = new List<float>();
            for (int i = 0; i < craftingHandler.Queues.Count; i++)
            {
                names.Add(craftingHandler.Queues[i].Item.Name);
                amounts.Add(craftingHandler.Queues[i].Amount);
                times.Add(craftingHandler.Queues[i].Timer);
                maxTimes.Add(craftingHandler.Queues[i].MaxTimer);
            }

            return new SaveData
            {
                itemName = names,
                itemAmount = amounts,
                itemTimer = times,
                itemMaxTimer = maxTimes
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            craftingHandler.Queues.Clear();
            for (int i = 0; i < saveData.itemName.Count; i++)
            {
                var item = itemDatabase.GetItem(saveData.itemName[i]);
                craftingHandler.Queues.Add(new QI_CraftingQueue { Item = item, Amount = saveData.itemAmount[i], Timer = saveData.itemTimer[i], MaxTimer = saveData.itemMaxTimer[i] });
            }
        }

        [Serializable]
        private struct SaveData
        {
            public List<string> itemName;
            public List<int> itemAmount;
            public List<float> itemTimer;
            public List<float> itemMaxTimer;

        }
    } 
}