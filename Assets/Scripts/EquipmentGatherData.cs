using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/Equipment Item/GathererItem", fileName = "New Gatherer Item")]
public class EquipmentGatherData : EquipmentData
{
    public QI_ItemData gatherItemData;
    

    

    public override void UseEquippedItem()
    {
        base.UseEquippedItem();
        
    }

}
