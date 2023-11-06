using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Klaxon.StatSystem;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/ConsumableItem", fileName = "New Consumable Item")]
public class ConsumableItemData : QI_ItemData
{

    public List<StatChanger> statChangers = new List<StatChanger>();
    public List<StatModifier> statModifiers = new List<StatModifier>();
    public QI_ItemData itemToBecome;

    public override void UseItem()
    {
        base.UseItem();
        ConsumeItem();
    }
    void ConsumeItem()
    {
        foreach (var mod in statModifiers)
        {
            PlayerInformation.instance.statHandler.AddModifier(mod);
        }
        foreach (var mod in statChangers)
        {
            PlayerInformation.instance.statHandler.ChangeStat(mod);
        }

        
        PlayerInformation.instance.playerInventory.RemoveItem(this.Name, 1);
        if(itemToBecome != null)
            PlayerInformation.instance.playerInventory.AddItem(itemToBecome, 1, false); 
    }
}
