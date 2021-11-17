using QuantumTek.EncryptedSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;
using System.IO;

public class SaveAndLoadAll : MonoBehaviour
{
    public static SaveAndLoadAll instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    QI_ItemDatabase allItemDatabase;

    List<string> allItems = new List<string>();
    List<string> allIDs = new List<string>();

    private void Start()
    {
        allItemDatabase = AllItemsDatabaseManager.instance.allItemsDatabase;
    }
    public void SaveAll()
    {
        //Figure this out, it ain't good... well...
        var s = Directory.GetParent(Application.persistentDataPath).FullName;
        string[] filePaths = Directory.GetFiles(s); 
        foreach (string filePath in filePaths) 
            File.Delete(filePath);

        allItems.Clear();
        allIDs.Clear();
        foreach (var manager in FindObjectsOfType<SaveableManager>())
        {

            manager.Save();
        }
        foreach (var item in FindObjectsOfType<SaveableItem>())
        {
            allItems.Add(item.item.Name);
            allIDs.Add(item.ID);
            item.Save();
        }
        ES_Save.Save(allItems, "AllItems");
        ES_Save.Save(allIDs, "AllIDs");
        
    }

    public void LoadAll()
    {

        allItemDatabase ??= AllItemsDatabaseManager.instance.allItemsDatabase;

        allItems.Clear();
        allIDs.Clear();
        allItems = ES_Save.Load<List<string>>("AllItems");
        allIDs = ES_Save.Load<List<string>>("AllIDs");

        foreach (var item in FindObjectsOfType<SaveableItem>())
        {
            Destroy(item.gameObject);
        }
        foreach (var manager in FindObjectsOfType<SaveableManager>())
        {

            manager.Load();
        }

        for (int i = 0; i < allItems.Count; i++)
        {
            var go = Instantiate(allItemDatabase.GetItem(allItems[i]).ItemPrefab);
            go.GetComponent<SaveableItem>().SetID(allIDs[i]);
            go.GetComponent<SaveableItem>().Load();
        }
        
    }

}
