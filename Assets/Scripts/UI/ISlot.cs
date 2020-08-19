using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlot
{
    void AddItem(QI_ItemData newItem, int amount);
    void RemoveItem();
    void ClearSlot();
    void TransferItem();

    void SetIndex(int index);
}
