using QuantumTek.QuantumInventory;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemsSaveSystem : MonoBehaviour, ISaveable
{

    public QI_ItemDatabase itemDatabase;
    


    public object CaptureState()
    {
        QI_Item[] items = FindObjectsOfType<QI_Item>();
        List<string> tempItem = new List<string>();
        List<string> tempItemID = new List<string>();
        foreach (var item in items)
        {
            if(item.TryGetComponent(out SaveableItemEntity entity))
            {
                
                tempItem.Add(item.Data.Name);
                tempItemID.Add(entity.ID);
            }
            
        }
        return new SaveData
        {
            items = tempItem,
            itemID = tempItemID
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        SaveableItemEntity[] items = FindObjectsOfType<SaveableItemEntity>();
        foreach (var item in items)
        {
            if(item.TryGetComponent(out QI_Item value))
                Destroy(item.gameObject);
        }



        for (int i = 0; i < saveData.items.Count; i++)
        {
            
            var entity = Instantiate(itemDatabase.GetItem(saveData.items[i]).ItemPrefab, transform.position, Quaternion.identity);
            if (entity.TryGetComponent(out SaveableItemEntity saveableItem))
                saveableItem.SetId(saveData.itemID[i]);
            
        }
            
            

        
    }

    [Serializable]
    private struct SaveData
    {
        public List<string> items;
        public List<string> itemID;

    }
}
