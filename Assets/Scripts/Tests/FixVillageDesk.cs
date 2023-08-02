using Klaxon.SAP;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FixVillageArea
{
    public string areaName;
    [TextArea(5, 20)]
    public string areaDescription;
    public Sprite areaIcon;
    public GameObject fixedArea;
    public int fixTime;
    [HideInInspector]
    public CycleTicks fixTicks;
    public int sparksRequired;
    public List<FixableAreaIngredient> ingredients = new List<FixableAreaIngredient>();
    public List<NavigationNode> navigationNodes = new List<NavigationNode>();
    public SAP_Condition SAP_Condition;
    public bool isFixing;
    public bool isActive;
}
public class FixVillageDesk : MonoBehaviour
{
    public List<FixVillageArea> fixableAreas = new List<FixVillageArea>();
    
    

    
}
