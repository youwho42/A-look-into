using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectPlayerVisibility : MonoBehaviour
{

    SurroundingTiles surroundingTiles;
    CurrentGridLocation currentGridLocation;
    public SpriteRenderer playerSpriteRenderer;
    public SpriteRenderer shadowSpriteRenderer;
    Vector3Int lastTilePosition;
    private void Start()
    {
        surroundingTiles = GetComponent<SurroundingTiles>();
        currentGridLocation = GetComponent<CurrentGridLocation>();
        lastTilePosition = currentGridLocation.lastTilePosition;
    }


    private void Update()
    {
        if(lastTilePosition != currentGridLocation.lastTilePosition)
        {
            CheckTiles();
        }
                  
    }

    void CheckTiles()
    {
        bool isHidden = false;
        foreach (var tile in surroundingTiles.allCurrentDirections)
        {
            if (tile.Key == Vector3Int.left || tile.Key == -(Vector3Int)Vector2Int.one || tile.Key == Vector3Int.down)
            {
                if (tile.Value.difference >= 1 && !tile.Value.isValid)
                {
                    isHidden = true;
                    //IsometricRuleTile currentRuleTile = currentGridLocation.groundMap.GetTile(currentGridLocation.lastTilePosition + tile.Key + Vector3Int.forward) as IsometricRuleTile;
                    
                }
            }
        }
        EnableMask(isHidden);
    }

    

    void EnableMask(bool isOn)
    {
        var mask = isOn ? SpriteMaskInteraction.VisibleOutsideMask : SpriteMaskInteraction.None;
        playerSpriteRenderer.maskInteraction = mask;
        shadowSpriteRenderer.maskInteraction = mask;

    }
    
}
