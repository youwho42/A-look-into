using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CurrentGridLocation : MonoBehaviour
{
    [HideInInspector]
    public Grid groundGrid;
    [HideInInspector]
    public Tilemap groundMap;

    [HideInInspector]
    public int currentLevel;

    float tileScale;
    int tilePosZ;

    [HideInInspector]
    public Vector3Int lastTilePosition;

    private void Start()
    {
        SetGrid();
        tileScale = Mathf.Abs((groundGrid.cellSize.y * -0.5f) - 0.01f);
        lastTilePosition = GetCurrentGridLocation();
    }
  
    public bool UpdateLocation()
    {
        tilePosZ = GetTileLocation();
        currentLevel = tilePosZ;
        Vector3Int tempPos = GetCurrentGridLocation();
        if (tempPos != lastTilePosition)
        {
            lastTilePosition = tempPos;
            return true;
        }
        return false;
    }

    public void UpdateLocationAndPosition()
    {
        UpdateLocation();
        transform.position = new Vector3(transform.position.x, transform.position.y, currentLevel);
    }

    public int GetTileLocation()
    {

        SetGrid();

        int tilesHit = 0;

        for (int i = groundMap.cellBounds.zMax; i > groundMap.cellBounds.zMin; i--)
        {
            lastTilePosition.z = i;
            TileBase tile = groundMap.GetTile(lastTilePosition);
            if (tile != null)
            {
                tilesHit = i+1;
                break;
            }
        }
        return tilesHit;
    }

    public Vector3Int GetCurrentGridLocation()
    {
        SetGrid();

        var pos = new Vector3(transform.position.x, transform.position.y, transform.position.z-1); // this is the self world position

        Vector3Int tilepos = groundGrid.WorldToCell(pos); // this is Tile grid position

        return tilepos;
        
    }

    void SetGrid()
    {
        if (groundGrid != null)
            return;
        
        groundGrid = FindObjectOfType<Grid>();
        Tilemap[] maps = groundGrid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            if (map.gameObject.name == "GroundTiles")
            {
                groundMap = map;
            }
        }
    }
}
