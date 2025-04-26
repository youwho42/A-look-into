using NUnit.Framework;
using QuantumTek.QuantumInventory;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Painting", menuName = "Klaxon/Painting")]
public class PaintingSO : ScriptableObject
{

    [Serializable]
    public struct PaintingLayer
    {
        public Sprite LayerSprite;
        public QI_ItemData itemNeeded;
        
    }
    public LocalizedString localizedName;
    public LocalizedString localizedDescription;
    public Sprite paintingBGSprite;
    public List<PaintingLayer> paintingLayers = new List<PaintingLayer>();
    public Sprite finalLayer;
}
