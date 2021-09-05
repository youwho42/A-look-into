using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CurrentGridLocation : MonoBehaviour
{
 
    public Grid groundGrid;
    public Tilemap groundMap;

    public int currentLevel;

    const float tileScale = 0.27808595f;

    public Vector3Int lastTilePosition;

    

   /* private void Update()
    {
        if (transform.position.z != GetTileLocation())
        {
            InitializeLocation();
        }
    }*/
    public void InitializeLocation()
    {
        lastTilePosition = GetCurrentGridLocation();
        currentLevel = GetTileLocation();
        transform.position = new Vector3(transform.position.x, transform.position.y, currentLevel);
    }

    public int GetTileLocation()
    {
        int tilesHit = 0;

        Vector3Int currentPosition = GetCurrentGridLocation();


        for (int i = 0; i < groundMap.size.z; i++)
        {
            currentPosition.z = i;
            TileBase tile = groundMap.GetTile(currentPosition);
            if (tile != null)
            {
                tilesHit++;
            }
        }
        return tilesHit;
    }

    public Vector3Int GetCurrentGridLocation()
    {
        Vector3 currentPosition = transform.position;
        currentPosition = new Vector3(currentPosition.x, currentPosition.y + (tileScale * (currentLevel - 1)), 0f);

        Vector3Int checkPosition = groundGrid.WorldToCell(currentPosition);

        return checkPosition;
    }


 
}
