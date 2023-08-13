using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

namespace Klaxon.SaveSystem
{
    public class MerchantTableSaveSystem : MonoBehaviour, ISaveable
    {

        public QI_ItemDatabase itemDatabase;
        public List<InteractableMerchantTable> merchantTables = new List<InteractableMerchantTable>();


        public object CaptureState()
        {
            List<string> names = new List<string>();
            List<int> amounts = new List<int>();
            foreach (var table in merchantTables)
            {
                names.Add(table.item.Name);
                amounts.Add(table.amount);
                Debug.Log(table.item.Name);
            }
            return new SaveData
            {
                itemNames = names,
                itemAmounts = amounts
            };
        }

        public void RestoreState(object state)
        {
            
            //var saveData = (SaveData)state;
            //for (int i = 0; i < merchantTables.Count; i++)
            //{
            //    if (saveData.itemAmounts[i] > 0)
            //        merchantTables[i].SetUpTable(itemDatabase.GetItem(saveData.itemNames[i]), saveData.itemAmounts[i]);
            //}
            
            
        }

        [Serializable]
        private struct SaveData
        {
            public List<string> itemNames;
            public List<int> itemAmounts;

        }
    }
}
