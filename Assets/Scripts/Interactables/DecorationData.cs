using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/DecorationItem", fileName = "New Decoration Item")]

public class DecorationData : QI_ItemData
{
    public List<Transform> variants = new List<Transform>();
}
