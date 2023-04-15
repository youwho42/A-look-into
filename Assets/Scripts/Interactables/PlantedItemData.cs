using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/PlantedItem", fileName = "New Planted Item")]
public class PlantedItemData : QI_ItemData
{
    [Serializable]
    public struct HarvestedItem
    {
        public QI_ItemData harvestedItem;
        public Vector2Int minMaxAmount;
        public AnimationCurve amountVariance;
    }

    public HarvestedItem[] harvestedItems;

    public int GetAmount(AnimationCurve variance, Vector2 minMaxAmount)
    {
        
        var t = (variance.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f))) * (minMaxAmount.y - minMaxAmount.x) + minMaxAmount.x;

        return (int)t;
    }
}
