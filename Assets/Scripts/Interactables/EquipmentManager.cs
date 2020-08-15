using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public EquipmentData[] currentEquipment;
    int totalSlots;
    public static EquipmentManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        totalSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new EquipmentData[totalSlots];
    }

    public void Equip(EquipmentData newItem)
    {
        int slotIndex = (int)newItem.equipmentSlot;
        currentEquipment[slotIndex] = newItem;
    }
    public void UnEquipToInventory(EquipmentData equipedItem)
    {
        int slotIndex = (int)equipedItem.equipmentSlot;
        currentEquipment[slotIndex] = null;
        PlayerInformation.instance.playerInventory.AddItem(equipedItem, 1);
    }

    public void UnEquipAndDestroy(EquipmentData equipedItem)
    {
        int slotIndex = (int)equipedItem.equipmentSlot;
        currentEquipment[slotIndex] = null;
    }
}
