using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Klaxon.Interactable;
using Klaxon.GOAD;
using UnityEngine.Tilemaps;

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
    public NavigationNode reachNode;
    public NavigationNode fixingNode;
    public GOAD_ScriptableCondition GOAD_AvailableCondition;
    public GOAD_ScriptableCondition GOAD_CompletedCondition;
    public UndertakingObject undertakingObject;
    public UndertakingTaskObject taskObject;
    [HideInInspector]
    public bool isFixing;
    [HideInInspector]
    public bool isActive;
    public Collider2D areaCollider;
    public List<Vector3Int> pathfindingTilePositions = new List<Vector3Int>();
}
public class FixVillageDesk : MonoBehaviour
{
    public List<FixVillageArea> fixableAreas = new List<FixVillageArea>();
    [HideInInspector]
    public bool isActive;
    public Tilemap groundTiles; 
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
            foreach (var tilePos in area.pathfindingTilePositions)
            {
                if(PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.TryGetValue(tilePos, out IsometricNodeXYZ value))
                    value.walkable = area.isActive;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var area in fixableAreas)
        {
            foreach (var tilePos in area.pathfindingTilePositions)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(groundTiles.GetCellCenterWorld(tilePos), 0.2f);
            }
        }

        
    }
}
