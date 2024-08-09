using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Klaxon.Interactable;
using Klaxon.GOAD;

[Serializable]
public class FixVillageArea
{
    public string areaName;
    public LocalizedString localizedName;
    public LocalizedString localizedDescription;
    
    public Sprite areaIcon;
    public GameObject inactiveArea;
    public GameObject activeArea;
    public FixingSounds fixingSounds;
    public ParticleSystem fixingEffect;
    public int ticksToFix;
    [HideInInspector]
    public int fixTimer;
    public int sparksRequired;
    public List<FixableAreaIngredient> ingredients = new List<FixableAreaIngredient>();
    public List<NavigationNode> navigationNodes = new List<NavigationNode>();
    public NavigationNode reachNode;
    public NavigationNode fixingNode;
    public GOAD_ScriptableCondition GOAD_CompletedCondition;
    public UndertakingObject undertakingObject;
    public UndertakingTaskObject taskObject;
    [HideInInspector]
    public bool isFixing;
    [HideInInspector]
    public bool isActive;
}
public class FixVillageDesk : MonoBehaviour
{
    public List<FixVillageArea> fixableAreas = new List<FixVillageArea>();
    [HideInInspector]
    public bool isActive;

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
