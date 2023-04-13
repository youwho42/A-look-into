using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/SeedItem", fileName = "New Seed Item")]
public class SeedItemData : QI_ItemData
{

    public float plantingDistance;

    public PlantedItemData plantedObject;

}
