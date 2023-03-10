using QuantumTek.QuantumInventory;
using QuantumTek.QuantumAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/ConsumableItem", fileName = "New Consumable Item")]
public class ConsumableItemData : QI_ItemData
{
    [Serializable]
    public struct AttributeModifier
    {
        public string attributeToModify;
        public float amount;
    }

    public List<AttributeModifier> attributeModifiers;
    public QI_ItemData itemToBecome;

    public override void UseItem()
    {
        base.UseItem();
        ConsumeItem();
    }
    void ConsumeItem()
    {
        for (int i = 0; i < attributeModifiers.Count; i++)
        {
            PlayerInformation.instance.playerStats.AddToStat(attributeModifiers[i].attributeToModify, attributeModifiers[i].amount);
        }
        PlayerInformation.instance.playerInventory.RemoveItem(this.Name, 1);
        if(itemToBecome != null)
            PlayerInformation.instance.playerInventory.AddItem(itemToBecome, 1, false); 
    }
}
