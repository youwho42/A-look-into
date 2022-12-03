using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[System.Serializable]
public class TileDirectionInfo
{
    public Vector3Int direction;
    public bool isValid;
    public int levelZ;
    public string tileName = "";

    public TileDirectionInfo(Vector3Int dir, bool valid, int z, string tile)
    {
        direction = dir;
        isValid = valid;
        levelZ = z;
        tileName = tile;
    }
}
[System.Serializable]
public class TileBlockInfo
{
    public Vector3Int tilePosition;
    public List<TileDirectionInfo> allTilesValues = new List<TileDirectionInfo>();
    public TileBlockInfo(Vector3Int pos, List<TileDirectionInfo> tileDirections)
    {
        tilePosition = pos;
        allTilesValues = new List<TileDirectionInfo>(tileDirections);
    }
}


public class GenerateAllTilesInfo : MonoBehaviour
{

    public Tilemap groundTileMap;
    public string allTilesName;


    public Vector3Int currentTilePosition;

    
    List<TileDirectionInfo> allDirectionsValues = new List<TileDirectionInfo>();
    [HideInInspector]
    public List<TileBlockInfo> allTilesValues = new List<TileBlockInfo>();

    public void ClearAllTiles()
    {
        allTilesValues.Clear();
        allDirectionsValues.Clear();
    }
    public void GetAllTiles()
    {


        
        for (int x = groundTileMap.cellBounds.min.x; x < groundTileMap.cellBounds.max.x; x++)
        {
            for (int y = groundTileMap.cellBounds.min.y; y < groundTileMap.cellBounds.max.y; y++)
            {
                for (int z = groundTileMap.cellBounds.min.z; z < groundTileMap.cellBounds.max.z; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    if (TileIsValid(pos))
                    {
                        currentTilePosition = pos;
                        allDirectionsValues.Clear();
                        GetAllSurroundingPositions();
                        TileBlockInfo newBlock = new TileBlockInfo(pos, allDirectionsValues);
                        allTilesValues.Add(newBlock);
                    }
                    
                   
                }
            }
        }
        Debug.Log("Tiles Saved Successfully");
        
    }

    bool TileIsValid(Vector3Int pos)
    {
        if(groundTileMap.GetTile(pos) == null/* || groundTileMap.GetTile(pos + Vector3Int.forward) != null*/)
            return false;

        return true;
    }

    void GetAllSurroundingPositions()
    {
        //allDirectionsValues.Clear();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {

                bool valid = false;

                var boundsMinZ = groundTileMap.localBounds.min.z;
                var diffZ = boundsMinZ - currentTilePosition.z;

                for (int z = (int)groundTileMap.localBounds.max.z; z >= diffZ; z--)
                {
                    Vector3Int currentPosition = currentTilePosition;
                    currentPosition.x += x;
                    currentPosition.y += y;
                    currentPosition.z += z;
                    TileBase tile = groundTileMap.GetTile(currentPosition);
                    string tileName = "Empty";

                    if (tile != null) // if tile exists here
                    {
                        tileName = tile.name;
                        if (x == 0 && y == 0) //this is where we are
                        {
                            if (z != 0)
                                continue;
                            valid = true;
                        }
                        if (z == -1) // we are on z-1
                        {
                            valid = tile.name.Contains("Slope");
                        }
                        else if (z == 0) // we are on z0
                        {
                            valid = true;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    else// the tile is null
                    {
                        if (z > 0)// we are on z1 or z2
                            valid = true;
                        else // we are on or below z0
                            valid = false;
                    }

                    


                    Vector3Int dir = new Vector3Int(x, y, 0);

                    var thing = allDirectionsValues.Find(item => item.direction == dir);
                    if (thing != null)
                    {
                        if (!thing.isValid)
                        {

                            if (z < 0)
                            {
                                if (valid || thing.tileName == "Empty")
                                {
                                    allDirectionsValues.Find(item => item.direction == dir).isValid = valid;
                                    allDirectionsValues.Find(item => item.direction == dir).levelZ = z;
                                    allDirectionsValues.Find(item => item.direction == dir).tileName = tileName;
                                }
                                    
                            }
                            else
                            {
                                continue;
                            }
                                
                        }
                        else
                        {
                            if (z < 0)
                                continue;
                            allDirectionsValues.Find(item => item.direction == dir).isValid = valid;
                            allDirectionsValues.Find(item => item.direction == dir).levelZ = z;
                            allDirectionsValues.Find(item => item.direction == dir).tileName = tileName;
                        }

                    }
                    else
                    {
                        allDirectionsValues.Add(new TileDirectionInfo(dir, valid, z, tileName));
                    }
                }
            }
        }
       

        // check if on slope, set respective slope direction z1 to valid
        foreach (var item in allDirectionsValues)
        {
            if(item.direction == Vector3Int.zero && item.tileName.Contains("Slope"))
            {
                Vector3Int tileDirection = item.tileName.Contains("X") ? Vector3Int.right : Vector3Int.up;
                tileDirection = item.tileName.Contains("1") ? tileDirection : -tileDirection;
                var thing = allDirectionsValues.Find(item => item.direction == tileDirection);
                thing.isValid = true;

                var otherThingA = allDirectionsValues.Find(item => item.direction == -SwapDirection(tileDirection));
                var otherThingB = allDirectionsValues.Find(item => item.direction == SwapDirection(tileDirection));
                otherThingA.isValid = false;
                otherThingB.isValid = false;

            }
        }
        
    }

    Vector3Int SwapDirection(Vector3Int item)
    {
        Vector3Int result = item;
        var temp = result.x;
        result.x = result.y;
        result.y = temp;
        return result;
    }
}
