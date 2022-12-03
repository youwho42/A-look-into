using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;
using System;

public class ContainerItemsAtStart : MonoBehaviour
{
    [Serializable]
    public struct ContainerSlotItem
    {
        public QI_ItemData itemData;
        public int amount;
    }

    QI_Inventory inventory;
    public List<ContainerSlotItem> slotItems = new List<ContainerSlotItem>();
    public bool addStockAtStart;

    private void Start()
    {
        if (addStockAtStart)
            AddStock();
    }


    [ContextMenu("Add Stock")]
    public void AddStock()
    {
        inventory = GetComponent<QI_Inventory>();
        foreach (var item in slotItems)
        {
            inventory.AddItem(item.itemData, item.amount, true);
        }
    }
}
