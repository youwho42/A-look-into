using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/ConsumableItem", fileName = "New Consumable Item")]
public class ConsumableItemData : QI_ItemData
{

    public override void UseItem()
    {
        base.UseItem();
        Debug.Log("Consuming Item");
        ConsumeItem();
    }
    void ConsumeItem()
    {

    }
}
