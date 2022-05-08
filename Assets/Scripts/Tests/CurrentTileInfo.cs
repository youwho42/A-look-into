using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTileInfo : MonoBehaviour
{
    
    AllTilesManager allTilesManager;
    Vector3Int currentPosition;
    bool onSlope;
    private void Start()
    {
        
        allTilesManager = AllTilesManager.instance;
        currentPosition = GetCurrentTilePos();
        Invoke("GetInfo", 1f);
    }
    
    void GetInfo()
    {
        var position = allTilesManager.GetTileInfo(currentPosition);
        
    }

    private void Update()
    {
        currentPosition = GetCurrentTilePos();
        var position = allTilesManager.GetTileInfo(currentPosition);
        if(position.TryGetValue(Vector3Int.zero, out DirectionInfo bob))
        {
            Debug.Log(bob.tileName);
        }
    }

    public Vector3Int GetCurrentTilePos()
    {
        
        Vector3Int cellIndex = allTilesManager.groundMap.WorldToCell(transform.position - Vector3.forward);
        /*for (int i = allTilesManager.groundMap.cellBounds.zMax; i > allTilesManager.groundMap.cellBounds.zMin - 1; i--)
        {
            cellIndex.z = i;
            var tile = allTilesManager.groundMap.GetTile(cellIndex);
            if (tile != null)
            {

                return cellIndex;
            }


        }*/

        return cellIndex;

    }



    /*public bool CanReachNextTile(Vector2 direction)
    {
        
        Vector3 checkPosition = (transform.position + (Vector3)direction * 0.08f) - Vector3.forward;
        Vector3 doubleCheckPosition = transform.position - Vector3.forward;
        *//*if (gravityItem.CheckForObstacles(checkPosition, doubleCheckPosition, direction))
            return false;*//*

        var nextTilePosition = gravityItem.surroundingTiles.grid.WorldToCell(checkPosition);

        Vector3Int nextTileKey = nextTilePosition - currentPosition;

        if (nextTileKey == Vector3Int.zero)
            return true;



        gravityItem.surroundingTiles.GetSurroundingTiles();

        int level = 0;

        foreach (var tile in gravityItem.surroundingTiles.allCurrentDirections)
        {
            // CURRENT TILE ----------------------------------------------------------------------------------------------------
            // right now, where we are, what it be? is it be a slope?
            if (tile.Key == Vector3Int.zero)
            {

                gravityItem.slopeDirection = Vector2.zero;
                onSlope = tile.Value.tileName.Contains("Slope");
                if (onSlope)
                {
                    if (tile.Value.tileName.Contains("X"))
                        gravityItem.slopeDirection = tile.Value.tileName.Contains("0") ? new Vector2(-0.9f, -0.5f) : new Vector2(0.9f, 0.5f);
                    else
                        gravityItem.slopeDirection = tile.Value.tileName.Contains("0") ? new Vector2(0.9f, -0.5f) : new Vector2(-0.9f, 0.5f);
                    continue;
                }

            }
            if (tile.Key == nextTileKey)
                level = tile.Value.levelZ;
            else
                continue;
            Vector3Int doubleCheckTilePosition = gravityItem.surroundingTiles.grid.WorldToCell(doubleCheckPosition);



            // JUMPING! ----------------------------------------------------------------------------------------------------
            // I don't care what height the tile is at as long as the sprite is jumping and has a y above the tile height
            if (tile.Key == nextTileKey)
            {

                if (Mathf.Abs(gravityItem.itemObject.localPosition.z) >= level)
                {
                    gravityItem.surroundingTiles.currentTilePosition += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

                    if (tile.Value.tileName.Contains("Slope"))
                        onSlope = true;

                    return true;
                }
            }

            // GROUNDED! ----------------------------------------------------------------------------------------------------
            // the next tile is valid



            if (tile.Key == nextTileKey && tile.Value.isValid)
            {

                // if the next tile is a slope, am i approaching it in the right direction?
                if (tile.Value.tileName.Contains("Slope"))
                {
                    if (tile.Value.tileName.Contains("X") && nextTileKey.x == 0 || tile.Value.tileName.Contains("Y") && nextTileKey.y == 0)
                        return false;

                    onSlope = true;

                    // is the slope is lower?
                    if (tile.Value.levelZ < 0)
                        gravityItem.getOnSlope = true;


                }

                // I am on a slope
                if (onSlope)
                {
                    //am i walking 'off' the slope on the upper part in the right direction?
                    if (gravityItem.surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x == 0 || gravityItem.surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y == 0)
                    {
                        //onCliffEdge = true;
                        return false;
                    }
                    if (tile.Value.levelZ > 0)
                        gravityItem.getOffSlope = true;
                }

            }

            // the next tile is NOT valid
            if (tile.Key == nextTileKey && !tile.Value.isValid)
            {
                if (doubleCheckTilePosition == nextTilePosition)
                {
                    gravityItem.Nudge(direction);
                }

                // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                if (onSlope)
                {
                    if (gravityItem.surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x != 0 || gravityItem.surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y != 0)
                        continue;
                }

                // This is where we are at he bottom of a cliff
                if (tile.Value.levelZ > 0)
                {
                    return true;
                }

                // This is where we hit a wall of height 1 or above
                return false;
            }
        }

        gravityItem.surroundingTiles.currentTilePosition += new Vector3Int(nextTileKey.x, nextTileKey.y, level);



        return true;
    }*/




}
