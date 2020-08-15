using QuantumTek.QuantumInventory;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveableWorldObjects : MonoBehaviour, ISaveable
{

    public QI_ItemDatabase itemDatabase;
    


    public object CaptureState()
    {
        QI_Item[] items = FindObjectsOfType<QI_Item>();
        List<string> tempItem = new List<string>();
        List<SVector3> tempLocations = new List<SVector3>();
        List<string> tempItemID = new List<string>();
        foreach (var item in items)
        {
            if(item.TryGetComponent(out SaveableEntity entity))
            {
                
                tempItem.Add(item.Data.Name);
                tempLocations.Add(item.transform.position);
                tempItemID.Add(entity.ID);
            }
            
        }
        return new SaveData
        {
            items = tempItem,
            locations = tempLocations
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        SaveableEntity[] items = FindObjectsOfType<SaveableEntity>();
        foreach (var item in items)
        {
            if(item.TryGetComponent(out QI_Item value))
                Destroy(item.gameObject);
        }



        for (int i = 0; i < saveData.items.Count; i++)
        {
            
            Instantiate(itemDatabase.GetItem(saveData.items[i]).ItemPrefab, saveData.locations[i], Quaternion.identity);
            
            
        }
            
            

        
    }

    [Serializable]
    private struct SaveData
    {
        public List<string> items;
        public List<SVector3> locations;
        public List<string> itemID;

    }
}
