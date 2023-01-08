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
        if(EquipmentManager.instance.currentEquipment[(int)equipmentSlot] != null)
        {
            PlayerInformation.instance.playerInventory.RemoveItem(this, 1);
            PlayerInformation.instance.playerInventory.AddItem(EquipmentManager.instance.currentEquipment[(int)equipmentSlot], 1, false);
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
    Light
}
