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
    public GameObject inactiveArea;
    public GameObject activeArea;
    public int ticksToFix;
    //[HideInInspector]
    public int fixTimer;
    public int sparksRequired;
    public List<FixableAreaIngredient> ingredients = new List<FixableAreaIngredient>();
    public List<NavigationNode> navigationNodes = new List<NavigationNode>();
    public NavigationNode fixingNode;
    public SAP_Condition SAP_CompletedCondition;
    public bool isFixing;
    public bool isActive;
}
public class FixVillageDesk : MonoBehaviour
{
    public List<FixVillageArea> fixableAreas = new List<FixVillageArea>();


    private void Start()
    {
        SetAllAreas();
    }

    public void SetAllAreas()
    {
        foreach (var area in fixableAreas)
        {
            area.activeArea.SetActive(area.isActive);
            area.inactiveArea.SetActive(!area.isActive);
        }
    }

}
