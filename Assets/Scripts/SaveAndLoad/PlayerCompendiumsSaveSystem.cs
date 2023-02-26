using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCompendiumsSaveSystem : MonoBehaviour, ISaveable
{

    public QI_ItemDatabase playerAnimalCompendiumDatabase;
    public QI_ItemDatabase playerResourceCompendiumDatabase;
    public QI_ItemDatabase playerNoteCompendiumDatabase;
    public QI_ItemDatabase playerGuidesCompendiumDatabase;
    public QI_ItemDatabase allItemsDatabase;
    public PlayerAnimalCompendiumInformation animalCompendiumInformation;
    public object CaptureState()
    {
        
        List<string> animalNames = new List<string>();
        List<string> resourcesNames = new List<string>();
        List<string> noteNames = new List<string>();
        List<string> guideNames = new List<string>();

        for (int i = 0; i < playerAnimalCompendiumDatabase.Items.Count; i++)
        {
            animalNames.Add(playerAnimalCompendiumDatabase.Items[i].Name);
        }
        for (int i = 0; i < playerResourceCompendiumDatabase.Items.Count; i++)
        {
            resourcesNames.Add(playerResourceCompendiumDatabase.Items[i].Name);
        }
        for (int i = 0; i < playerNoteCompendiumDatabase.Items.Count; i++)
        {
            noteNames.Add(playerNoteCompendiumDatabase.Items[i].Name);
        }
        for (int i = 0; i < playerGuidesCompendiumDatabase.Items.Count; i++)
        {
            guideNames.Add(playerGuidesCompendiumDatabase.Items[i].Name);
        }

        return new SaveData
        {
            animalItemName = animalNames,
            resourceItemName = resourcesNames,
            noteItemName = noteNames,
            guideItemName = guideNames,
            animalCompendiumNames = animalCompendiumInformation.animalNames,
            animalCompendiumAmounts = animalCompendiumInformation.animalTimesViewed
            
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        playerAnimalCompendiumDatabase.Items.Clear();
        playerResourceCompendiumDatabase.Items.Clear();
        playerNoteCompendiumDatabase.Items.Clear();
        playerGuidesCompendiumDatabase.Items.Clear();
        for (int i = 0; i < saveData.animalItemName.Count; i++)
        {
            playerAnimalCompendiumDatabase.Items.Add(allItemsDatabase.GetItem(saveData.animalItemName[i]));
        }
        for (int i = 0; i < saveData.resourceItemName.Count; i++)
        {
            playerResourceCompendiumDatabase.Items.Add(allItemsDatabase.GetItem(saveData.resourceItemName[i]));
        }
        for (int i = 0; i < saveData.noteItemName.Count; i++)
        {
            playerNoteCompendiumDatabase.Items.Add(allItemsDatabase.GetItem(saveData.noteItemName[i]));
        }
        for (int i = 0; i < saveData.guideItemName.Count; i++)
        {
            playerGuidesCompendiumDatabase.Items.Add(allItemsDatabase.GetItem(saveData.guideItemName[i]));
        }
        animalCompendiumInformation.animalNames = saveData.animalItemName;
        animalCompendiumInformation.animalTimesViewed = saveData.animalCompendiumAmounts;
        GameEventManager.onAnimalCompediumUpdateEvent.Invoke();
        GameEventManager.onResourceCompediumUpdateEvent.Invoke();
        GameEventManager.onNoteCompediumUpdateEvent.Invoke();
        GameEventManager.onGuideCompediumUpdateEvent.Invoke();
    }

    [Serializable]
    private struct SaveData
    {
        public List<string> animalItemName;
        public List<string> resourceItemName;
        public List<string> noteItemName;
        public List<string> guideItemName;
        public List<string> animalCompendiumNames;
        public List<int> animalCompendiumAmounts;

    }

}