using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/PlantedItem", fileName = "New Planted Item")]
public class PlantedItemData : QI_ItemData
{
    [Serializable]
    public struct HarvestedItem
    {
        public QI_ItemData harvestedItem;
        public int harvestedAmount;
    }

    public HarvestedItem[] harvestedItems;
}
