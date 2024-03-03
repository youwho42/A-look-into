using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    Grid grid;
    public Tilemap groundMap;
    public Grid Grid { get { return grid == null ?SetGrid() : grid; } }

    Grid SetGrid()
    {
        if (grid != null)
            return grid;

        grid = FindObjectOfType<Grid>();
        Tilemap[] maps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            if (map.gameObject.name == "GroundTiles")
            {
                groundMap = map;
            }
        }
        return grid;
    }

    public bool GetTileValid(Vector3 position)
    {
        if (grid == null || groundMap == null)
            SetGrid();
        var tile = groundMap.WorldToCell(position) - Vector3Int.forward;
        var tileAbove = tile + Vector3Int.forward;
        if (groundMap.GetTile(tile) != null && groundMap.GetTile(tileAbove) == null)
            return true;
        return false;
    }
}
