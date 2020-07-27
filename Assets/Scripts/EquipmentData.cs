using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/EquipmentItem", fileName = "New Equipment Item")]
public class EquipmentData : QI_ItemData
{
    public EquipmentSlot equipmentSlot;
    public override void UseItem()
    {
        base.UseItem();
        EquipItem();
    }
    void EquipItem()
    {
        EquipmentManager.instance.Equip(this);
    }
}
public enum EquipmentSlot { Hands, Head, Feet, Legs, Chest, Extra }
