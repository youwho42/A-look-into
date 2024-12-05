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

    public Tilemap decorationOne;
    public Tilemap decorationTwo;


    Grid SetGrid()
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
            if (map.gameObject.name == "GroundTileDetails")
                decorationOne = map;
            if (map.gameObject.name == "GroundTileDetails2")
                decorationTwo = map;
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
                    if (groundMap.GetTile(pos) != null)
                    {
                        groundMap.SetTileFlags(pos, TileFlags.None);
                        decorationOne.SetTileFlags(pos, TileFlags.None);
                        decorationTwo.SetTileFlags(pos, TileFlags.None);

                        float c = NumberFunctions.RemapNumber(z, -1, groundMap.cellBounds.zMax, 0.0f, 0.25f);
                        Color levelcolor = Color.Lerp(Color.white, Color.black, c);
                        groundMap.SetColor(pos, levelcolor);
                        decorationOne.SetColor(pos, levelcolor);
                        decorationTwo.SetColor(pos, levelcolor);

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
        Vector2 randPos = Random.insideUnitCircle * maxDistance;
        Vector3 offsetPos = origin + (Vector3)randPos;
        
        Vector3Int destinationTile = groundMap.WorldToCell(offsetPos - Vector3Int.forward);
        for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin; z--)
        {
            destinationTile.z = z;
            if (GetTileValid(destinationTile))
                return groundMap.GetCellCenterWorld(destinationTile) + Vector3.forward;
        }
        return origin;
    }

    public Vector3Int GetRandomNodeInArea(Vector3Int startPos, int maxDistance)
    {
        SetGrid();
        int x = Random.Range(-maxDistance, maxDistance);
        int y = Random.Range(-maxDistance, maxDistance);
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

}
