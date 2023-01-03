using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricGridXYZ : MonoBehaviour
{

    Grid grid;
    [HideInInspector]
    public Tilemap groundMap;

    [HideInInspector]
    public List<IsometricNodeXYZ> isometricNodes = new List<IsometricNodeXYZ>();

    public TerrainType[] terrainTypes;
    LayerMask walkableMask;
    public LayerMask obstacleMask;
    Dictionary<int, int> terrainDictionary = new Dictionary<int, int>();
    public int maxSize;
    BoundsInt groundBounds;
    private void Start()
    {
        SetGrid();
        SetNodes();
        maxSize = MaxSize;
        groundBounds = groundMap.cellBounds;
        foreach (TerrainType terrain in terrainTypes)
        {
            walkableMask.value |= terrain.terrainMask.value;
            terrainDictionary.Add((int)Mathf.Log(terrain.terrainMask.value, 2), terrain.terrainPenalty);

        }
    }
    public int MaxSize
    {
        get { return groundMap.size.x + groundMap.size.y + groundMap.size.z; }
    }

    public List<IsometricNodeXYZ> GetNeighbours(IsometricNodeXYZ node)
    {
        // Here somewhere I check the z level
        // if z level is lower by one, is it a slope?
        // if z level is higher by one, am I on a slope?

        List<IsometricNodeXYZ> neighbours = new List<IsometricNodeXYZ>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {

                    if (x == 0 && y == 0 || Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                        continue;

                    int checkX = node.worldPosition.x + x;
                    int checkY = node.worldPosition.y + y;
                    int checkZ = node.worldPosition.z + z;

                    if (checkX >= groundBounds.xMin && checkX <= groundBounds.xMax
                        && checkY >= groundBounds.yMin && checkY <= groundBounds.yMax
                        && checkZ >= groundBounds.zMin && checkZ <= groundBounds.zMax)
                    {
                        neighbours.Add(GetIsometricNode(new Vector3Int(checkX, checkY, checkZ)));
                    }
                }
                    
            }
        }

        return neighbours;
    }

    public IsometricNodeXYZ GetIsometricNode(Vector3Int position)
    {

        foreach (var n in isometricNodes)
        {
            if (n.worldPosition == position)
                return n;
        }
        return null;
    }

    void SetNodes()
    {
        for (int z = groundMap.cellBounds.zMin; z <= groundMap.cellBounds.zMax; z++)
        {
            for (int x = groundMap.cellBounds.xMin; x <= groundMap.cellBounds.xMax; x++)
            {
                for (int y = groundMap.cellBounds.yMin; y <= groundMap.cellBounds.yMax; y++)
                {
                
                    bool walkable = true;
                    bool isSlope = false;
                    var currentPosition = new Vector3Int(x, y, z);
                    string slopeName = "";
                    TileBase tile = groundMap.GetTile(currentPosition);

                    if (tile == null)
                    {
                        // there is no tile here
                        walkable = false;
                        continue;
                    }
                    else if (tile != null)
                    {
                        if (tile.name.Contains("Slope"))
                        {
                            isSlope = true;
                            slopeName = tile.name;
                        }
                            
                        //there is a tile, but is there one above it?
                        TileBase tileAbove = groundMap.GetTile(currentPosition + Vector3Int.forward);
                        if (tileAbove != null)
                            walkable = false;
                    }

                    int movementPenalty = 1000;

                    if (walkable)
                    {
                        for (int i = 0; i < terrainTypes.Length; i++)
                        {
                            Collider2D hit = Physics2D.OverlapCircle(GetTileWorldPosition(currentPosition), 0.3f, terrainTypes[i].terrainMask);
                            if (hit != null)
                            {
                                movementPenalty = terrainTypes[i].terrainPenalty;
                            }
                        }
                        Collider2D obstacleCheck = Physics2D.OverlapCircle(GetTileWorldPosition(currentPosition), 0.03f, obstacleMask);
                        if (obstacleCheck != null)
                        {
                            walkable = false;
                        }

                    }
                    
                    isometricNodes.Add(new IsometricNodeXYZ(walkable, new Vector3Int(x, y, z), Mathf.Abs(groundMap.cellBounds.xMin - x), Mathf.Abs(groundMap.cellBounds.yMin - y), Mathf.Abs(groundMap.cellBounds.zMin - z), movementPenalty, isSlope, slopeName));
                }
            }
        }
    }

    public Vector3 GetTileWorldPosition(Vector3Int tile)
    {

        var tileworldpos = groundMap.GetCellCenterWorld(tile);
        return tileworldpos;
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

    [Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}