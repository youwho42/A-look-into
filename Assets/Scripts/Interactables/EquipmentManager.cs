using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentManager : MonoBehaviour
{
    public QI_ItemData[] currentEquipment;
    int totalSlots;
    public static EquipmentManager instance;
    public UnityEvent EventUIUpdateEquipment;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        totalSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new QI_ItemData[totalSlots];
    }

    public void Equip(QI_ItemData newItem, int equipedIndex)
    {
        currentEquipment[equipedIndex] = newItem;
        EventUIUpdateEquipment.Invoke();
    }
  
    public void UnEquipToInventory(QI_ItemData itemData, int equipedIndex)
    {
        currentEquipment[equipedIndex] = null;
        PlayerInformation.instance.playerInventory.AddItem(itemData, 1);
        EventUIUpdateEquipment.Invoke();
    }

    public void UnEquipAndDestroy(int equipedIndex)
    {
        currentEquipment[equipedIndex] = null;
        EventUIUpdateEquipment.Invoke();
    }
    public void UnEquipAndDestroyAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            currentEquipment[i] = null;
        }
        EventUIUpdateEquipment.Invoke();
    }
}
