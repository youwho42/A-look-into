using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    Grid grid;
    public Tilemap groundMap;
    public Tilemap waterMap;
    public Grid Grid { get { return grid == null ? SetGrid() : grid; } }

    

    [Serializable]
    public struct MapTerrains
    {
        public List<Tilemap> tilemaps;
        public bool useHalftone;
    }

    public List<MapTerrains> mapTerrains = new List<MapTerrains>();

    public Grid SetGrid()
    {
        if (grid != null && groundMap != null)
            return grid;

        grid = FindFirstObjectByType<Grid>();
        Tilemap[] maps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            if (map.gameObject.name == "GroundTiles")
                groundMap = map;
            if (map.gameObject.name == "WaterTiles")
                waterMap = map;
            //    if (map.gameObject.name == "GroundTileDetails")
            //        decorationOne = map;
            //    if (map.gameObject.name == "GroundTileDetails2")
            //        decorationTwo = map;
            //    if (map.gameObject.name == "Beach")
            //        beachMap = map;
            //    if (map.gameObject.name == "BeachDetails")
            //        beachDetailsMap = map;
        }
        return grid;
    }
    private void Start()
    {
        GameEventManager.onGameLoadedEvent.AddListener(SetGradient);
        GameEventManager.onGameStartLoadEvent.AddListener(SetGradient);
        SetGradient();
    }

    private void OnDestroy()
    {
        GameEventManager.onGameLoadedEvent.RemoveListener(SetGradient);
        GameEventManager.onGameStartLoadEvent.RemoveListener(SetGradient);
    }

    void SetGradient()
    {
        SetGrid();
        for (int x = groundMap.cellBounds.xMin; x < groundMap.cellBounds.xMax; x++)
        {
            for (int y = groundMap.cellBounds.yMin; y < groundMap.cellBounds.yMax; y++)
            {
                for (int z = -1; z < groundMap.cellBounds.zMax; z++)
                {
                Vector3Int pos = new Vector3Int(x, y, z);
                    float c = NumberFunctions.RemapNumber(z, -1, groundMap.cellBounds.zMax, 0.0f, 0.25f);
                    Color levelcolor = Color.Lerp(Color.white, Color.black, c);
                    Color levelcolorB = Color.Lerp(Color.white, Color.black, c * 0.5f);
                    if (groundMap.GetTile(pos) != null)
                    {
                        groundMap.SetTileFlags(pos, TileFlags.None);
                        groundMap.SetColor(pos, levelcolor);
                    }
                    foreach (var terrain in mapTerrains)
                    {
                        foreach (var map in terrain.tilemaps)
                        {
                            if (map.GetTile(pos) == null)
                                continue;
                            map.SetTileFlags(pos, TileFlags.None);
                            map.SetColor(pos, terrain.useHalftone ? levelcolorB : levelcolor);
                        }
                    }
                }
            }
        }
    }

    public bool GetTileValid(Vector3 position)
    {
        SetGrid();

        var tile = groundMap.WorldToCell(position - Vector3Int.forward);
        var tileAbove = tile + Vector3Int.forward;
        if (groundMap.GetTile(tile) != null && groundMap.GetTile(tileAbove) == null)
            return true;
        return false;
    }

    public bool GetTileValid(Vector3Int tile)
    {
        SetGrid();
        var tileAbove = tile + Vector3Int.forward;
        for (int i = groundMap.cellBounds.zMax; i < groundMap.cellBounds.zMin; i--)
        {
            var t = new Vector3Int(tile.x, tile.y, i);
            if (waterMap.GetTile(tile) != null)
                return false;
        }
        if (groundMap.GetTile(tile) != null && groundMap.GetTile(tileAbove) == null)
            return true;
        return false;
    }

    public bool GetTileExisting(Vector3 position)
    {
        SetGrid();

        var tile = groundMap.WorldToCell(position - Vector3Int.forward);
        //var tileAbove = tile + Vector3Int.forward;
        if (groundMap.GetTile(tile) != null)
            return true;
        return false;
    }

    public Vector3Int GetTilePosition(Vector3 position)
    {
        SetGrid();
        return groundMap.WorldToCell(position - Vector3Int.forward);
    }

    public Vector3 GetTileWorldPosition(Vector3Int position)
    {
        SetGrid();
        return groundMap.GetCellCenterWorld(position);
    }




    public Vector3 GetRandomTileWorldPosition(Vector3 origin, float maxDistance)
    {
        SetGrid();
        Vector2 randPos = UnityEngine.Random.insideUnitCircle * maxDistance;
        Vector3 offsetPos = origin + (Vector3)randPos;
        
        Vector3Int destinationTile = groundMap.WorldToCell(offsetPos - Vector3Int.forward);
        for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin; z--)
        {
            destinationTile.z = z;
            if (GetTileValid(destinationTile))
                return groundMap.GetCellCenterWorld(destinationTile) + Vector3.forward;
        }
        return GetRandomTileWorldPosition(origin, maxDistance);
    }

    public Vector3 GetRandomTileWorldPosition(Vector3 origin, float minDistance, float maxDistance)
    {
        SetGrid();
       
        Vector2 randPos = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(minDistance, maxDistance);
        Vector3 offsetPos = origin + (Vector3)randPos;

        Vector3Int destinationTile = groundMap.WorldToCell(offsetPos - Vector3Int.forward);
        for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin; z--)
        {
            destinationTile.z = z;
            if (GetTileValid(destinationTile))
                return groundMap.GetCellCenterWorld(destinationTile) + Vector3.forward;
        }
        return GetRandomTileWorldPosition(origin, maxDistance);
    }

    public Vector3Int GetRandomNodeInArea(Vector3Int startPos, int maxDistance)
    {
        SetGrid();
        int x = UnityEngine.Random.Range(-maxDistance, maxDistance);
        int y = UnityEngine.Random.Range(-maxDistance, maxDistance);
        if (x == 0 && y == 0)
            x = 1;
        var endPosition = new Vector3Int(startPos.x + x, startPos.y + y, startPos.z);
        for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin; z--)
        {
            endPosition.z = z;
            if(PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.TryGetValue(endPosition, out IsometricNodeXYZ nodeXYZ))
            {
                if (nodeXYZ.walkable)
                    return endPosition;
            }
        }
        return startPos;
    }

    public bool HasWaterTile(Vector3Int tile)
    {
        SetGrid();
        
        if (waterMap.GetTile(tile) != null)
            return true;
        return false;
    }

    
    public void CompressAllTilemapBounds()
    {
        
        grid = FindFirstObjectByType<Grid>();
        Tilemap[] maps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            map.CompressBounds();
            
        }
        
    }

}
