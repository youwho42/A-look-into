using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDisplaySlot : MonoBehaviour, ISlot
{
    public QI_ItemData item;
    public Image icon;
    public TextMeshProUGUI itemSlotName;
    public TextMeshProUGUI itemUse;
    


    public EquipmentSlot equipmentSlot;


    public void ShowItemUse(bool active)
    {
        
        if (item != null)
            itemUse.text = active ? "Unequip" : "";

    }


    public void TransferItem()
    {
        if (item != null)
        {
            if(EquipmentManager.instance.UnEquipToInventory(item, (int)equipmentSlot))
                ClearSlot();
        }

    }

    public void AddItem(QI_ItemData newItem, int amount)
    {
        item = newItem;
        icon.sprite = item.Icon;
        icon.enabled = true;

    }

    public void RemoveItem()
    {
        if (item == null)
            return;

        
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        itemUse.text = "";
    }

    public void SetIndex(int index)
    {
        equipmentSlot = (EquipmentSlot)index;
        itemSlotName.text = equipmentSlot.ToString();
    }

}
