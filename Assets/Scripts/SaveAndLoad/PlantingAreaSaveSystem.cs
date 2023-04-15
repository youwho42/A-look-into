using QuantumTek.QuantumInventory;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlantingAreaSaveSystem : MonoBehaviour, ISaveable
{

    public PlantingArea plantingArea;
    public QI_ItemDatabase allItemsDatabase;
    public object CaptureState()
    {
        List<SVector3> free = new List<SVector3>();
        List<SVector3> used = new List<SVector3>();
        for (int i = 0; i < plantingArea.plantFreeLocations.Count; i++)
        {
            free.Add(plantingArea.plantFreeLocations[i]);
        }
        for (int i = 0; i < plantingArea.plantUsedLocations.Count; i++)
        {
            free.Add(plantingArea.plantUsedLocations[i]);
        }
        string n = plantingArea.seedItem != null ? plantingArea.seedItem.Name : "";
        return new SaveData
        {
            freeLocations = free,
            usedLocations = used,
            seedItem = n

        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        if(saveData.seedItem != "")
            plantingArea.seedItem = allItemsDatabase.GetItem(saveData.seedItem) as SeedItemData;
        for (int i = 0; i < saveData.freeLocations.Count; i++)
        {
            plantingArea.plantFreeLocations.Add(saveData.freeLocations[i]);
        }
        for (int i = 0; i < saveData.usedLocations.Count; i++)
        {
            plantingArea.plantUsedLocations.Add(saveData.usedLocations[i]);
        }
        plantingArea.CheckForPlantable();

    }

    [Serializable]
    private struct SaveData
    {
        public List<SVector3> freeLocations;
        public List<SVector3> usedLocations;
        public string seedItem;
    }
}
