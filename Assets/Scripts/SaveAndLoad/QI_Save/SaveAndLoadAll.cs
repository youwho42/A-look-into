using QuantumTek.EncryptedSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

public class SaveAndLoadAll : MonoBehaviour
{
    public QI_ItemDatabase allItemsDatabase;

    List<string> allItems = new List<string>();
    List<string> allIDs = new List<string>();

    public void SaveItems()
    {
        allItems.Clear();
        allIDs.Clear();
        foreach (var item in FindObjectsOfType<SaveableItem>())
        {
            allItems.Add(item.item.Name);
            allIDs.Add(item.ID);
            item.Save();
        }
        ES_Save.Save(allItems, "AllItems");
        ES_Save.Save(allIDs, "AllIDs");
    }

    public void LoadItems()
    {
        allItems.Clear();
        allIDs.Clear();
        allItems = ES_Save.Load<List<string>>("AllItems");
        allIDs = ES_Save.Load<List<string>>("AllIDs");

        foreach (var item in FindObjectsOfType<SaveableItem>())
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < allItems.Count; i++)
        {
            var go = Instantiate(allItemsDatabase.GetItem(allItems[i]).ItemPrefab);
            go.GetComponent<SaveableItem>().SetID(allIDs[i]);
            go.GetComponent<SaveableItem>().Load();
        }
        
    }

}
