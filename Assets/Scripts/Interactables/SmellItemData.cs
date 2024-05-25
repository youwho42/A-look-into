using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/SmellItem", fileName = "New Smell Item")]
public class SmellItemData : QI_ItemData
{
    [ColorUsage(true, true)]
    public Color smellEmissionColor;
    public Color smellColor;
}
