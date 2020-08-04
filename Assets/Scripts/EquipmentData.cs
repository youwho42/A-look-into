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
        Debug.Log("Equipping Item");
        EquipItem();
    }
    void EquipItem()
    {
        EquipmentManager.instance.Equip(this);
    }
    public virtual void UseEquippedItem()
    {
        Debug.Log("Using Equipped Item");
    }
}
public enum EquipmentSlot { Hands, Head, Feet, Legs, Chest, Extra }
