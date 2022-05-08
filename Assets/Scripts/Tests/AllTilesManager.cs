using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AllTilesManager : MonoBehaviour
{

    public static AllTilesManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this; 
        else
            Destroy(this);
    }


    SurroundingTilesInfo surroundingTiles;

    public Dictionary<Vector3Int, Dictionary<Vector3Int, DirectionInfo>> allTiles = new Dictionary<Vector3Int, Dictionary<Vector3Int, DirectionInfo>>();
    public Tilemap groundMap;
    BoundsInt bounds;

    Dictionary<Vector3Int, DirectionInfo> allCurrentDirections = new Dictionary<Vector3Int, DirectionInfo>();

    private void Start()
    {
        bounds = groundMap.cellBounds;
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                for (int z = bounds.zMin; z <= bounds.zMax; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    if (GetTileValidity(pos))
                    {
                        allCurrentDirections.Clear();
                        GetAllSurroundingPositions(pos);
                        allTiles.Add(pos, allCurrentDirections);
                    }
                }
            }
        }
    }

    public Dictionary<Vector3Int, DirectionInfo> GetTileInfo(Vector3Int position)
    {
        
        if (allTiles.ContainsKey(position))
        {
            return allTiles[position];
        }
        return null;
    }

    bool GetTileValidity(Vector3Int tilePosition)
    {

        bool isValid = false;
        for (int i = 0; i <= 2; i++)
        {
            tilePosition.z += i;
            var tile = groundMap.GetTile(tilePosition);
            if (tile != null)
            {
                if (i == 0)
                    isValid = true;
                else
                    isValid = false;

            }

        }

        return isValid;
    }

    void GetAllSurroundingPositions(Vector3Int currentTilePosition)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {

                bool valid = false;

                var boundsMinZ = groundMap.localBounds.min.z;
                var diffZ = boundsMinZ - currentTilePosition.z;

                for (int z = 2; z >= diffZ; z--)
                {
                    Vector3Int currentPosition = currentTilePosition;
                    currentPosition.x += x;
                    currentPosition.y += y;
                    currentPosition.z += z;
                    TileBase tile = groundMap.GetTile(currentPosition);
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


                    if (allCurrentDirections.ContainsKey(new Vector3Int(x, y, 0)))
                    {
                        if (!allCurrentDirections[new Vector3Int(x, y, 0)].isValid)
                        {

                            if (z < 0)
                            {
                                if (valid || allCurrentDirections[new Vector3Int(x, y, 0)].tileName == "Empty")
                                    allCurrentDirections[new Vector3Int(x, y, 0)] = new DirectionInfo(valid, z, tileName);
                            }
                            else
                                continue;
                        }
                        else
                        {
                            if (z < 0)
                                continue;
                            allCurrentDirections[new Vector3Int(x, y, 0)] = new DirectionInfo(valid, z, tileName);
                        }

                    }
                    else
                    {
                        allCurrentDirections.Add(new Vector3Int(x, y, 0), new DirectionInfo(valid, z, tileName));
                    }

                }

            }
        }

        // check if on slope, set respective slope direction z1 to valid
        if (allCurrentDirections[Vector3Int.zero].tileName.Contains("Slope"))
        {

            Vector3Int tileDirection = allCurrentDirections[Vector3Int.zero].tileName.Contains("X") ? Vector3Int.right : Vector3Int.up;
            tileDirection = allCurrentDirections[Vector3Int.zero].tileName.Contains("1") ? tileDirection : -tileDirection;
            allCurrentDirections[tileDirection].isValid = true;

        }
    }

}
