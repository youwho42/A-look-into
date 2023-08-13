using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class EquipmentSaveSystem : MonoBehaviour, ISaveable
    {

        public QI_ItemDatabase itemDatabase;

        public object CaptureState()
        {
            var equipment = EquipmentManager.instance;
            List<string> names = new List<string>();
            List<int> index = new List<int>();
            for (int i = 0; i < equipment.currentEquipment.Length; i++)
            {
                if (equipment.currentEquipment[i] != null)
                {
                    names.Add(equipment.currentEquipment[i].Name);
                    index.Add(i);
                }

            }

            return new SaveData
            {
                itemName = names,
                itemIndex = index
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            EquipmentManager.instance.UnEquipAndDestroyAll();
            for (int i = 0; i < saveData.itemName.Count; i++)
            {

                EquipmentManager.instance.Equip(itemDatabase.GetItem(saveData.itemName[i]), saveData.itemIndex[i]);

            }
        }

        [Serializable]
        private struct SaveData
        {
            public List<string> itemName;
            public List<int> itemIndex;

        }
    }

}