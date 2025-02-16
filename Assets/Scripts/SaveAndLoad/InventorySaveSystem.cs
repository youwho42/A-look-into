using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class InventorySaveSystem : MonoBehaviour, ISaveable
    {
        public QI_Inventory inventory;
        public QI_ItemDatabase itemDatabase;


        public object CaptureState()
        {
            List<string> names = new List<string>();
            List<int> amounts = new List<int>();
            for (int i = 0; i < inventory.Stacks.Count; i++)
            {
                names.Add(inventory.Stacks[i].Item.Name);
                amounts.Add(inventory.Stacks[i].Amount);
            }

            return new SaveData
            {
                maxStacks = inventory.MaxStacks,
                itemName = names,
                itemAmount = amounts
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            inventory.MaxStacks = saveData.maxStacks;
            inventory.RemoveAllItems();
            for (int i = 0; i < saveData.itemName.Count; i++)
            {

                inventory.AddItem(itemDatabase.GetItem(saveData.itemName[i]), saveData.itemAmount[i], true);

            }
        }

        [Serializable]
        private struct SaveData
        {
            public int maxStacks;
            public List<string> itemName;
            public List<int> itemAmount;

        }
    }

}