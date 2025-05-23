﻿using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class EquipmentDisplaySlot : MonoBehaviour, ISlot
{
    public QI_ItemData item;
    public Image icon;
    public TextMeshProUGUI itemSlotName;
    public TextMeshProUGUI itemUse;
    
    

    public EquipmentSlot equipmentSlot;

    void OnEnable()
    {
        SetSlotName();
    }

    public void ShowItemUse(bool active)
    {
        string word = LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", "Unequip");
        if (item != null)
            itemUse.text = active ? word : "";

    }

    public void ShowInformation()
    {
        if (item == null)
            return;
        ItemInformationDisplayUI.instance.ShowItemName(item, this.GetComponent<RectTransform>());
    }
    public void HideInformation()
    {
        ItemInformationDisplayUI.instance.HideItemName();
    }

    public void TransferItem()
    {
        if (item != null)
        {
            if(equipmentSlot == EquipmentSlot.Backpack)
            {
                if (EquipmentManager.instance.UnequipBackpack(item, (int)equipmentSlot))
                    ClearSlot();
                else 
                    Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", "Inventory Full"), null, 0, NotificationsType.Warning);

                return;
            }
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
        SetSlotName();
        
    }

    void SetSlotName()
    {
        string word = LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", equipmentSlot.ToString());
        itemSlotName.text = word;
    }
}
