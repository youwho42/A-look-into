using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.EncryptedSave;
using QuantumTek.QuantumInventory;


public class SavePlayerInventory : SaveableManager
{
    QI_ItemDatabase allItemDatabase;
    QI_Inventory inventory;

    List<string> inventoryItems = new List<string>();
    List<int> inventoryQuantities = new List<int>();

    private void Start()
    {
        allItemDatabase = AllItemsDatabaseManager.instance.allItemsDatabase;
        inventory = GetComponent<QI_Inventory>();
    }
    public override void Save()
    {
        inventoryItems.Clear();
        inventoryQuantities.Clear();

        for (int i = 0; i < inventory.Stacks.Count; i++)
        {
            inventoryItems.Add(inventory.Stacks[i].Item.Name);
            inventoryQuantities.Add(inventory.Stacks[i].Amount);
        }
        
        ES_Save.Save(inventoryItems, saveableName + "Items");
        ES_Save.Save(inventoryQuantities, saveableName + "Quantities");
    }
    public override void Load()
    {
        base.Load();
        allItemDatabase ??= AllItemsDatabaseManager.instance.allItemsDatabase;
        StartCoroutine(LoadCo());
    }


    IEnumerator LoadCo()
    {
        inventoryItems.Clear();
        inventoryQuantities.Clear();
        inventory = GetComponent<QI_Inventory>();
        
        inventoryItems = ES_Save.Load<List<string>>(saveableName + "Items");
        inventoryQuantities = ES_Save.Load<List<int>>(saveableName + "Quantities");

        yield return new WaitForSeconds(.01f);

        inventory.RemoveAllItems();

        for (int i = 0; i < inventoryItems.Count; i++)
        {

            inventory.AddItem(allItemDatabase.GetItem(inventoryItems[i]), inventoryQuantities[i]);

        }


    }
}
