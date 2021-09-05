using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class PlayerLevelChange : MonoBehaviour
{


    public Transform playerFeetPosition;
    public Grid groundGrid;
    public Tilemap groundMap;

    public int playerLevel;

    public UnityEvent playerChangeLevelEvent;

    float tileScale;
    int tilePosZ;

    Vector3Int lastTilePosition;

    private void Start()
    {
        tileScale = (groundGrid.cellSize.y * -0.5f) - 0.01f;
        if (playerChangeLevelEvent == null)
        {
            playerChangeLevelEvent = new UnityEvent();
        }
        InitializePlayerLocation();


    }
    private void Update()
    {

        if (lastTilePosition != CurrentGridLocation())
        {
            tilePosZ = GetTileLocation();
            lastTilePosition = CurrentGridLocation();

        }


        if (playerLevel != tilePosZ && Mathf.Abs(playerLevel - tilePosZ) == 1)
        {
            playerLevel = tilePosZ;



            transform.position = new Vector3(transform.position.x, transform.position.y, playerLevel);
            playerChangeLevelEvent.Invoke();
        }

    }



    int GetTileLocation()
    {
        int tilesHit = 0;

        Vector3Int currentPosition = CurrentGridLocation();


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

    Vector3Int CurrentGridLocation()
    {
        Vector3 playerPosition = playerFeetPosition.position;
        playerPosition = new Vector3(playerPosition.x, playerPosition.y + (tileScale * (playerLevel - 1)), 0f);

        Vector3Int checkPosition = groundGrid.WorldToCell(playerPosition);

        return checkPosition;
    }



    public void InitializePlayerLocation()
    {
        lastTilePosition = CurrentGridLocation();
        playerLevel = GetTileLocation();

        playerChangeLevelEvent.Invoke();
    }


}
