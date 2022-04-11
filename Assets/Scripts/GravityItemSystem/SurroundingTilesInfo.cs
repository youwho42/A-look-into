using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SurroundingTilesInfo : MonoBehaviour
{
    [HideInInspector]
    public Grid grid;
    Tilemap groundMap;
    public Vector3Int currentTilePosition;

    public class DirectionInfo
    {
        public bool isValid;
        public int levelZ;
        public string tileName = "";

        public DirectionInfo(bool valid, int z, string tile)
        {
            isValid = valid;
            levelZ = z;
            tileName = tile;
        }
    }

    

    public Dictionary<Vector3Int, DirectionInfo> allCurrentDirections = new Dictionary<Vector3Int, DirectionInfo>();

    private void Start()
    {
        currentTilePosition = CheckCurrentTilePosition();
        GetSurroundingTiles();
    }

    

    public void GetSurroundingTiles()
    {
        SetGrid();

        allCurrentDirections.Clear();

        GetAllSurroundingPositions();
    }


    void GetAllSurroundingPositions()
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
                                if(valid || allCurrentDirections[new Vector3Int(x, y, 0)].tileName == "Empty")
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

    public Vector3 GetTileWorldPosition(Vector3Int tile)
    {
        var tileworldpos = groundMap.GetCellCenterWorld(tile);
        return tileworldpos;
    }

    private Vector3Int CheckCurrentTilePosition()
    {
        SetGrid();
        return grid.WorldToCell(transform.position - Vector3.forward);
    }


    private void OnDrawGizmosSelected()
    {
        if (allCurrentDirections.Count > 0)
        {
            foreach (var item in allCurrentDirections)
            {
                Gizmos.color = item.Value.isValid ? Color.green : Color.red;

                var p = item.Key + currentTilePosition;
                Gizmos.DrawWireSphere(GetTileWorldPosition(p), 0.1f);
            }
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(GetTileWorldPosition(currentTilePosition), 0.1f);
        }
    }

    void SetGrid()
    {
        if (grid != null)
            return;

        grid = FindObjectOfType<Grid>();
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