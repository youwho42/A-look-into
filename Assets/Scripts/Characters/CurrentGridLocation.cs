using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CurrentGridLocation : MonoBehaviour
{

    public Transform groundPosition;
    public Grid groundGrid;
    public Tilemap groundMap;

    public int currentLevel;

    float tileScale;
    int tilePosZ;

    public Vector3Int lastTilePosition;


    private void Start()
    {
        SetGrid();

        tileScale = (groundGrid.cellSize.y * -0.5f) - 0.01f;
        UpdateLocation();
    }
  

    public void UpdateLocation()
    {
        lastTilePosition = GetCurrentGridLocation();
        tilePosZ = GetTileLocation();
        currentLevel = tilePosZ;
        
        //transform.position = new Vector3(transform.position.x, transform.position.y, currentLevel);
    }
    public void UpdateLocationAndPosition()
    {
        lastTilePosition = GetCurrentGridLocation();
        tilePosZ = GetTileLocation();
        currentLevel = tilePosZ;
        transform.position = new Vector3(transform.position.x, transform.position.y, currentLevel);
    }

    

    public int GetTileLocation()
    {

        SetGrid();



        int tilesHit = 0;

        Vector3Int currentPosition = GetCurrentGridLocation();


       

        for (int i = groundMap.cellBounds.zMax; i > groundMap.cellBounds.zMin; i--)
        {
            currentPosition.z = i;
            TileBase tile = groundMap.GetTile(currentPosition);
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
        Vector3 currentPosition = groundPosition.position;
        currentPosition = new Vector3(currentPosition.x, currentPosition.y + (tileScale * (currentLevel - 1)), 0f);

        Vector3Int checkPosition = groundGrid.WorldToCell(currentPosition);

        return checkPosition;
    }


    void SetGrid()
    {
        if (groundGrid == null)
        {
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
 

}
