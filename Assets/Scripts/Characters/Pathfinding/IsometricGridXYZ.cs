using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricGridXYZ : MonoBehaviour
{

    Grid grid;
    [HideInInspector]
    public Tilemap groundMap;

    
    public Dictionary<Vector3Int, IsometricNodeXYZ> nodeLookup = new Dictionary<Vector3Int, IsometricNodeXYZ>();


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


        //    // Here somewhere I check the z level
        //    // if z level is lower by one, is it a slope?
        //    // if z level is higher by one, am I on a slope?

        //    List<IsometricNodeXYZ> neighbours = new List<IsometricNodeXYZ>();
        //    Vector3Int[] directions = {
        //    new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        //    new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
        //    new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)
        //};

        //    foreach (var dir in directions)
        //    {
        //        Vector3Int checkPosition = node.worldPosition + dir;
        //        if (groundBounds.Contains((Vector3Int)checkPosition))
        //        {
        //            IsometricNodeXYZ neighborNode = GetIsometricNode(checkPosition);
        //            if (neighborNode != null)
        //            {
        //                neighbours.Add(neighborNode);
        //            }
        //        }
        //    }

        //    return neighbours;
    }

    public IsometricNodeXYZ GetIsometricNode(Vector3Int position)
    {
        if (nodeLookup.TryGetValue(position, out IsometricNodeXYZ node))
        {
            return node;
        }
        return null;
        //foreach (var n in isometricNodes)
        //{
        //    if (n.worldPosition == position)
        //        return n;
        //}
        //return null;
    }

    void SetNodes()
    {
        for (int z = groundMap.cellBounds.zMin; z <= groundMap.cellBounds.zMax; z++)
        {
            for (int x = groundMap.cellBounds.xMin; x <= groundMap.cellBounds.xMax; x++)
            {
                for (int y = groundMap.cellBounds.yMin; y <= groundMap.cellBounds.yMax; y++)
                {

                    var currentPosition = new Vector3Int(x, y, z);
                    TileBase tile = groundMap.GetTile(currentPosition);

                    if (tile == null)
                    {
                        // There is no tile here
                        continue;
                    }

                    bool walkable = true;
                    bool isSlope = false;
                    string slopeName = "";
                    if (tile.name.Contains("Slope"))
                    {
                        isSlope = true;
                        slopeName = tile.name;
                    }

                    // Check for tile above
                    TileBase tileAbove = groundMap.GetTile(currentPosition + Vector3Int.forward);
                    if (tileAbove != null)
                    {
                        walkable = false;
                    }

                    int movementPenalty = 1000;
                    if (walkable)
                    {
                        movementPenalty = GetMovementPenalty(currentPosition);
                        
                        Collider2D obstacleCheck = Physics2D.OverlapCircle(GetTileWorldPosition(currentPosition), 0.1f, obstacleMask, currentPosition.z+1, currentPosition.z+1);
                        if (obstacleCheck != null)
                        {
                            walkable = false;
                        }
                    }
                    nodeLookup[currentPosition] = new IsometricNodeXYZ(walkable, currentPosition, x, y, z, movementPenalty, isSlope, slopeName);

                    //isometricNodes.Add(new IsometricNodeXYZ(walkable, currentPosition, x, y, z, movementPenalty, isSlope, slopeName));
                }
            }
        }
    }
    int GetMovementPenalty(Vector3Int position)
    {
        foreach (var terrain in terrainTypes)
        {
            Collider2D hit = Physics2D.OverlapCircle(GetTileWorldPosition(position), 0.3f, terrain.terrainMask);
            if (hit != null)
            {
                return terrain.terrainPenalty;
            }
        }
        return 1000;
    }

    public Vector3 GetTileWorldPosition(Vector3Int tile)
    {

        var tileworldpos = groundMap.GetCellCenterWorld(tile);
        return tileworldpos + Vector3.forward;
    }

    void SetGrid()
    {
        if (grid != null)
            return;

        grid = FindFirstObjectByType<Grid>();
        Tilemap[] maps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            if (map.gameObject.name == "GroundTiles")
            {
                groundMap = map;
            }
        }

    }

    private void OnDrawGizmosSelected()
    {

        foreach (var tile in nodeLookup)
        {
            if (!tile.Value.walkable)
                continue;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(GetTileWorldPosition(tile.Value.worldPosition), 0.1f);
        }

    }

    [Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}