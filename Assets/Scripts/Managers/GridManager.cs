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
        if (grid != null && groundMap != null)
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
        SetGrid();

        var tile = groundMap.WorldToCell(position - Vector3Int.forward);
        var tileAbove = tile + Vector3Int.forward;
        if (groundMap.GetTile(tile) != null && groundMap.GetTile(tileAbove) == null)
            return true;
        return false;
    }

    public bool GetTileValid(Vector3Int tile)
    {
        SetGrid();
        var tileAbove = tile + Vector3Int.forward;
        if (groundMap.GetTile(tile) != null && groundMap.GetTile(tileAbove) == null)
            return true;
        return false;
    }

    public bool GetTileExisting(Vector3 position)
    {
        SetGrid();

        var tile = groundMap.WorldToCell(position - Vector3Int.forward);
        var tileAbove = tile + Vector3Int.forward;
        if (groundMap.GetTile(tile) != null)
            return true;
        return false;
    }

    public Vector3 GetRandomTileWorldPosition(Vector3 origin, float maxDistance)
    {
        SetGrid();
        Vector2 randPos = Random.insideUnitCircle * maxDistance;
        Vector3 offsetPos = origin + (Vector3)randPos;
        
        Vector3Int destinationTile = groundMap.WorldToCell(offsetPos - Vector3Int.forward);
        for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin; z--)
        {
            destinationTile.z = z;
            if (GetTileValid(destinationTile))
                return groundMap.GetCellCenterWorld(destinationTile) + Vector3.forward;
        }
        return origin;
    }
}
