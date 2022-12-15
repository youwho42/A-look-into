using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricGrid : MonoBehaviour
{
    
    Grid grid;
    [HideInInspector]
    public Tilemap groundMap;

    [HideInInspector]
    public List<IsometricNode> isometricNodes = new List<IsometricNode>();

    public TerrainType[] terrainTypes;
    LayerMask walkableMask;
    Dictionary<int, int> terrainDictionary = new Dictionary<int, int>();

    private void Start()
    {
        SetGrid();
        SetNodes();

        foreach (TerrainType terrain in terrainTypes)
        {
            walkableMask.value |= terrain.terrainMask.value;
            terrainDictionary.Add((int)Mathf.Log(terrain.terrainMask.value, 2), terrain.terrainPenalty);
            
        }
    }
    public int MaxSize
    {
        get { return groundMap.size.x + groundMap.size.y; }
    }

    public List<IsometricNode> GetNeighbours(IsometricNode node)
    {
        // Here somewhere I check the z level
        // if z level is lower by one, is it a slope?
        // if z level is higher by one, am I on a slope?

        List<IsometricNode> neighbours = new List<IsometricNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0 || Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                    continue;

                int checkX = node.worldPosition.x + x;
                int checkY = node.worldPosition.y + y;

                if(checkX >= groundMap.cellBounds.xMin && checkX < groundMap.cellBounds.xMax
                    && checkY >= groundMap.cellBounds.yMin && checkY < groundMap.cellBounds.yMax)
                {
                    neighbours.Add(GetIsometricNode(new Vector3Int(checkX, checkY, 0)));
                }
            }
        }

        return neighbours;
    }

    public IsometricNode GetIsometricNode(Vector3Int position)
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
        
        for (int x = groundMap.cellBounds.xMin; x <= groundMap.cellBounds.xMax; x++)
        {
            for (int y = groundMap.cellBounds.yMin; y <= groundMap.cellBounds.yMax; y++)
            {
                // add for loop on groundMap.cellBounds.zMin/zMax
                bool walkable = true;
                var currentPosition = new Vector3Int(x, y, 0);
                // maybe... z = -1; z < 2; z++ - - well, no i think...
                for (int z = 0; z < 2; z++)
                {
                    currentPosition.z = z;
                    TileBase tile = groundMap.GetTile(currentPosition);
                    
                    if (tile == null && z == 0) // if the neighbor is lower
                    {
                        // for going down a level...
                        // need to check slopes as walkable on z == -1 -> -1
                        //else
                        walkable = false;
                    }
                    else if(tile != null && z == 1) // if the neighbor is higher
                    {
                        // for going up a level...
                        // need to check slopes as walkable on z == 0 -> +1
                        //else
                        walkable = false;
                    }
                    
                }
                int movementPenalty=100;

                
                if (walkable)
                {
                    Collider2D hit = Physics2D.OverlapCircle(GetTileWorldPosition(currentPosition), 0.1f, terrainTypes[0].terrainMask);
                    if(hit != null)
                    {

                        movementPenalty = 0;

                        
                    }
                    
                }
                
                isometricNodes.Add(new IsometricNode(walkable, new Vector3Int(x,y,0), Mathf.Abs(groundMap.cellBounds.xMin-x), Mathf.Abs(groundMap.cellBounds.yMin - y), movementPenalty));
                
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
