using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Klaxon.SaveSystem
{
    public class LatestVersionItemsSaveSystem : MonoBehaviour, ISaveable
    {

        public QI_ItemDatabase itemDatabase;



        public object CaptureState()
        {
            QI_Item[] items = FindObjectsOfType<QI_Item>();
            List<string> tempItem = new List<string>();
            List<string> tempItemID = new List<string>();
            List<string> tempVersion = new List<string>();
            List<int> tempItemVariant = new List<int>();
            foreach (var item in items)
            {
                if (item.TryGetComponent(out SaveableItemEntity entity))
                {
                    if(entity.version != "")
                    {
                        tempItem.Add(item.Data.Name);
                        tempItemID.Add(entity.ID);
                        tempVersion.Add(entity.version);
                        tempItemVariant.Add(item.itemVariantIndex);
                        
                    }
                    
                }

            }
            return new SaveData
            {
                items = tempItem,
                itemID = tempItemID,
                version = tempVersion,
                itemVariantIndex = tempItemVariant
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;

            var savedVersion = VersionDisplay.instance.intVersion;

            for (int i = 0; i < saveData.items.Count; i++)
            {
                var itemData = itemDatabase.GetItem(saveData.items[i]);
                if (itemData == null)
                    continue;
                var itemObject = itemData.ItemPrefabVariants[saveData.itemVariantIndex[i]];
                if (itemObject == null)
                    continue;

                
                if (!IsNewerVersion(savedVersion, saveData.version[i]))
                    continue;

                var entity = Instantiate(itemObject, transform.position, Quaternion.identity);
                if (entity.TryGetComponent(out SaveableItemEntity saveableItem))
                {
                    saveableItem.SetId(saveData.itemID[i]);
                    saveableItem.SetVersionFromSave(saveData.version[i]);
                }

                if (entity.TryGetComponent(out QI_Item Item))
                    Item.itemVariantIndex = saveData.itemVariantIndex[i];
            }
        }

        bool IsNewerVersion(int[] savedVersion, string itemV)
        {
            int[] itemVersion = itemV.Split('.').Select(s => int.Parse(s)).ToArray();
            for (int i = 0; i < savedVersion.Length; i++)
            {
                if (itemVersion[i] > savedVersion[i])
                    return true;
            }
            return false;
        }

        [Serializable]
        private struct SaveData
        {
            public List<string> items;
            public List<string> itemID;
            public List<string> version;
            public List<int> itemVariantIndex;

        }
    }

}
