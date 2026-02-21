using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[Serializable]
public class MapTextureArea
{
    public Texture2D MapTexture;
    [HideInInspector]
    public Color[,] MapColor;

    public void SetColorArray()
    {
        MapColor = ConvertImage();
        
    }

    Color[,] ConvertImage()
    {

        Color[,] convertedImage = new Color[MapTexture.width, MapTexture.height];
        for (int x = 0; x < MapTexture.width; x++)
        {
            for (int y = 0; y < MapTexture.height; y++)
            {
                
                convertedImage[x, y] = MapTexture.GetPixel(x, y);
               
            }
        }

       

        return convertedImage;
    }
}

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        foreach (var map in mapTextureAreas)
        {
            map.SetColorArray();
        }
    }

    Grid grid;
    public Tilemap groundMap;
    public Tilemap waterMap;
    public Tilemap houseRearMap;
    public Grid Grid { get { return grid == null ? SetGrid() : grid; } }

    

    [Serializable]
    public struct MapTerrains
    {
        public List<Tilemap> tilemaps;
        public bool useHalftone;
    }

    public List<MapTerrains> mapTerrains = new List<MapTerrains>();

    public List<MapTextureArea> mapTextureAreas = new List<MapTextureArea>();

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
            if (map.gameObject.name == "HousesRear")
                houseRearMap = map;
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
        for (int i = groundMap.cellBounds.zMax; i > groundMap.cellBounds.zMin; i--)
        {
            var t = new Vector3Int(tile.x, tile.y, i);
            if (waterMap.GetTile(tile) != null)
                return false;
        }
        if (groundMap.GetTile(tile) != null && groundMap.GetTile(tileAbove) == null)
            return true;
        return false;
    }

    public bool TryGetTileValid(Vector2Int tile, out Vector3Int outPos)
    {
        SetGrid();
        outPos = new Vector3Int(tile.x, tile.y, -1000);

       
        int mapPositionX = (int)NumberFunctions.RemapNumber(tile.x, groundMap.cellBounds.xMin, groundMap.cellBounds.xMax, 0, 128);
        int mapPositionY = (int)NumberFunctions.RemapNumber(tile.y, groundMap.cellBounds.yMin, groundMap.cellBounds.yMax, 0, 128);
        if (!CheckPositionAgainstMapTextures(mapPositionX, mapPositionY))
            return false;
        
        var t = new Vector3Int(tile.x, tile.y, 1000);
        for (int i = groundMap.cellBounds.zMax; i > groundMap.cellBounds.zMin; i--)
        {
            t.z = i;
            if (waterMap.GetTile(t) != null)
                return false;
            if (groundMap.GetTile(t) != null)
            {
                outPos = t;
                return true;
            }
               
        }
        return false;
    }



    public bool GetTileExisting(Vector3 position)
    {
        SetGrid();

        var tile = groundMap.WorldToCell(position - Vector3Int.forward);
        if (groundMap.GetTile(tile) != null)
            return true;
        return false;
    }

    public bool GetTileExisting(Tilemap map, Vector3 position)
    {
        SetGrid();

        var tile = map.WorldToCell(position - Vector3Int.forward);
        if (map.GetTile(tile) != null)
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

    public bool GetRandomTile(out Vector3 worldPosition)
    {
        SetGrid();

        int rx = UnityEngine.Random.Range(groundMap.cellBounds.xMin, groundMap.cellBounds.xMax);
        int ry = UnityEngine.Random.Range(groundMap.cellBounds.yMin, groundMap.cellBounds.yMax);

        int mapPositionX = (int)NumberFunctions.RemapNumber(rx, groundMap.cellBounds.xMin, groundMap.cellBounds.xMax, 0, 128);
        int mapPositionY = (int)NumberFunctions.RemapNumber(ry, groundMap.cellBounds.yMin, groundMap.cellBounds.yMax, 0, 128);

        Vector3Int destinationTile = new Vector3Int(rx, ry);
        worldPosition = Vector3.negativeInfinity;
        for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin; z--)
        {
            destinationTile.z = z;
            if (GetTileValid(destinationTile))
            {

                if (groundMap.GetTile(destinationTile).name.Contains("Slope"))
                    return false;
                if (!CheckPositionAgainstMapTextures(mapPositionX, mapPositionY))
                    return false;
                worldPosition = groundMap.GetCellCenterWorld(destinationTile) + Vector3.forward;
                return true;
            }
        }
        return false;
    }

    
    bool CheckPositionAgainstMapTextures(int x, int y)
    {
        if (x < 0 || x > 127 || y < 0 || y > 127)
            return false;
        foreach (var map in mapTextureAreas)
        {
            if (map.MapColor[x, y].a > 0.0f)
                return false; 
               
        }
        return true;
    }

    public Vector3 GetRandomTileWorldPosition(Vector3 origin, float maxDistance, bool limitZ = false)
    {
        SetGrid();
        Vector2 randPos = UnityEngine.Random.insideUnitCircle * maxDistance;
        Vector3 offsetPos = origin + (Vector3)randPos;
        
        Vector3Int destinationTile = groundMap.WorldToCell(offsetPos - Vector3Int.forward);
        for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin; z--)
        {
            if (limitZ)
            {
                if (Mathf.Abs(origin.z - z) > 2)
                    continue;
            }
            
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

    
    public Vector3 GetRandomGridBaseTile(Vector3Int origin, float maxDistance)
    {
        SetGrid();
        Vector3 finalPos = Vector3.negativeInfinity;
        List<Vector3Int> tiles = GetAllNearbyValidTiles(origin, maxDistance);
        if (tiles.Count > 0)
        {
            int r = UnityEngine.Random.Range(0, tiles.Count);
            finalPos = GetTileWorldPosition(tiles[r]);
        }
        return finalPos;

    }

    List<Vector3Int> GetAllNearbyValidTiles(Vector3Int origin, float maxDistance)
    {
        Vector2Int pos = (Vector2Int)origin;
        int distance = Mathf.CeilToInt(maxDistance * 2);
        List<Vector3Int> tiles = new List<Vector3Int>();

        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                pos.x = origin.x + x;
                pos.y = origin.y + y;

                if (TryGetTileValid(pos, out Vector3Int p))
                {
                    if (p.z >= origin.z - 1 && p.z <= origin.z + 1)
                        tiles.Add(p);
                }
            }
        }

        return tiles;
    }

    public List<Vector3> GetAllNearbyValidWorldPositions(Vector3Int origin, float maxDistance)
    {
        List<Vector3Int> tiles = GetAllNearbyValidTiles(origin, maxDistance);
        List<Vector3> positions = new List<Vector3>();

        foreach (Vector3Int t in tiles)
            positions.Add(GetTileWorldPosition(t));
        
        return positions;
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
