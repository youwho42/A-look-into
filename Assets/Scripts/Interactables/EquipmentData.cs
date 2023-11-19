using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/EquipmentItem", fileName = "New Equipment Item")]
public class EquipmentData : QI_ItemData
{

    public EquipmentSlot equipmentSlot;

    public EquipmentTier equipmentTier;
    
    public override void UseItem()
    {
        base.UseItem();
        EquipItem();
    }
    void EquipItem()
    {
        if(EquipmentManager.instance.currentEquipment[(int)equipmentSlot] != null)
        {
            PlayerInformation.instance.playerInventory.RemoveItem(this, 1);
            EquipmentManager.instance.UnEquipToInventory(EquipmentManager.instance.currentEquipment[(int)equipmentSlot], (int)equipmentSlot);
            EquipmentManager.instance.Equip(this, (int)equipmentSlot);
        } 
        else
        {
            PlayerInformation.instance.playerInventory.RemoveItem(this, 1);
            EquipmentManager.instance.Equip(this, (int)equipmentSlot);
        }

    }
    public override void UseEquippedItem()
    {
        base.UseEquippedItem();
    }
}
public enum EquipmentSlot 
{ 
    Hands, 
    Light,
    Compass
}

public enum EquipmentTier
{
    I,
    II,
    III
}
