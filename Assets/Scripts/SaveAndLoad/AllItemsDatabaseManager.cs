using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

public class AllItemsDatabaseManager : MonoBehaviour
{
    public static AllItemsDatabaseManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
            
    }

    public QI_ItemDatabase allItemsDatabase;

    public void ResetItemsDatabase()
    {
        allItemsDatabase.SetAllItems();
    }
}
