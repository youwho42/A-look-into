using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CurrentTilePosition : MonoBehaviour
{
    [HideInInspector]
    public Grid grid;
    [HideInInspector]
    public Tilemap groundMap;
    public Vector3Int position;

    private void Start()
    {
        SetGrid();
        position = GetCurrentTilePosition(transform.position);
    }
    public Vector3Int GetCurrentTilePosition(Vector3 worldPosition)
    {
        SetGrid();

        Vector3Int cellIndex = groundMap.WorldToCell(worldPosition - Vector3.forward);
        for (int i = groundMap.cellBounds.zMax; i > groundMap.cellBounds.zMin - 1; i--)
        {
            cellIndex.z = i;
            var tile = groundMap.GetTile(cellIndex);
            if (tile != null)
            {

                return cellIndex;
            }


        }

        return cellIndex;
    }

    public bool SetCurrentTilePosition(Vector3 worldPosition)
    {
        SetGrid();
        Vector3Int cellIndex = groundMap.WorldToCell(worldPosition - Vector3.forward);
        var tile = groundMap.GetTile(cellIndex);
        if (tile != null)
        {
            position = cellIndex;
            return true;
        }
        return false; 
        
    }


    void SetGrid()
    {
        if (grid != null)
            return;

        grid = FindFirstObjectByType<Grid>();
        Tilemap[] maps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            if (map.gameObject.name == "GroundTiles")
            {
                groundMap = map;
            }
        }
    }


}
