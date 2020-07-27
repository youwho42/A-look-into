using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    EquipmentData[] currentEquipment;
    int totalSlots;
    public static EquipmentManager instance;
    private void Start()
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
}
