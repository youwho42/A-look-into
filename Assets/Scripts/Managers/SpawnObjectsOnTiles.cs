using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[Serializable]
public class VectorQuadTree
{
    public Bounds bounds;
    public int capacity;
    public List<Vector3> allSpots = new List<Vector3>();
    public bool divided;
    public VectorQuadTree northWest;
    public VectorQuadTree northEast;
    public VectorQuadTree southWest;
    public VectorQuadTree southEast;

    public VectorQuadTree(Bounds bounds, int capacity)
    {
        this.bounds = bounds;
        this.capacity = capacity;
    }

    public void Insert(Vector3 spot)
    {
        if (!bounds.Contains(spot))
            return;

        if (allSpots.Count < capacity)
            allSpots.Add(spot);
        else
        {
            if (!divided)
                Subdivide();


            northEast.Insert(spot);
            northWest.Insert(spot);
            southWest.Insert(spot);
            southEast.Insert(spot);
        }

    }



    public void Subdivide()
    {
        Bounds nw = new Bounds(new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y + bounds.extents.y / 2), bounds.extents);
        northWest = new VectorQuadTree(nw, capacity);
        Bounds ne = new Bounds(new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y + bounds.extents.y / 2), bounds.extents);
        northEast = new VectorQuadTree(ne, capacity);
        Bounds sw = new Bounds(new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y - bounds.extents.y / 2), bounds.extents);
        southWest = new VectorQuadTree(sw, capacity);
        Bounds se = new Bounds(new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y - bounds.extents.y / 2), bounds.extents);
        southEast = new VectorQuadTree(se, capacity);

        divided = true;
    }


    public List<Vector3> QueryTree(Bounds boundry)
    {

        List<Vector3> spots = new List<Vector3>();

        if (!bounds.Intersects(boundry))
            return spots;

        foreach (var spot in allSpots)
        {
            if (boundry.Contains(spot))
                spots.Add(spot);

        }

        if (divided)
        {
            spots.AddRange(northWest.QueryTree(boundry));
            spots.AddRange(northEast.QueryTree(boundry));
            spots.AddRange(southWest.QueryTree(boundry));
            spots.AddRange(southEast.QueryTree(boundry));
        }

        return spots;
    }
}


public class SpawnObjectsOnTiles : MonoBehaviour
{
    Tilemap groundTilemap;
    
    public int maxObjectsPerTile;
    public SpawnObjectsPool pool;

    [Space]
    [Header("Noise")]
    public int mapSize = 128;
    public bool useOpenSimplex;

    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;


    public int seed;
    public Vector2 offset;

    
    public Bounds baseBounds = new Bounds(new Vector3(13, -10, 0), new Vector3(128, 128, 128));
    VectorQuadTree quadTree;
    //List<Vector3> objectPositions = new List<Vector3>();
    Bounds bounds;
    
    private void Start()
    {
        groundTilemap = GridManager.instance.groundMap;
        quadTree = new VectorQuadTree(baseBounds, 10);
        CreateNoiseMap();
        
        SetObjectsVisible();
       
        GameEventManager.onPlayerPositionUpdateEvent.AddListener(SetObjectsVisible);
    }

    private void OnDisable()
    {
        GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(SetObjectsVisible);
    }

    void SetObjectsVisible()
    {
        bounds = new Bounds(PlayerInformation.instance.player.position, new Vector3(10, 10, 16));
        var currentObjects = QueryQuadTree(bounds);
        pool.CheckAllObjectsVisible();
        foreach (var pos in currentObjects)
        {
            pool.SetObjectVisible(pos);
        }
        
    }


    public void CreateNoiseMap()
    {
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(useOpenSimplex, mapSize, mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        GenerateObjects(noiseMap);
    }

    void GenerateObjects(float[,] map)
    {
        
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                int mapX = (int)MapNumber.Remap(x, 0, mapSize, groundTilemap.cellBounds.xMin, groundTilemap.cellBounds.xMax);
                int mapY = (int)MapNumber.Remap(y, 0, mapSize, groundTilemap.cellBounds.yMin, groundTilemap.cellBounds.yMax);
                float mass = map[x, y];

                int possible = Mathf.RoundToInt(maxObjectsPerTile * mass);
                

                if (possible != 0)
                {
                    for (int z = groundTilemap.cellBounds.zMax; z >= groundTilemap.cellBounds.zMin; z--)
                    {
                        var p = new Vector3Int(mapX, mapY, z);
                        if (groundTilemap.GetTile(p) != null)
                        {
                            Vector3 pos = groundTilemap.GetCellCenterWorld(p);
                            
                            for (int i = 0; i <= possible; i++)
                            {

                                var finalPos = pos + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f), 1);
                                var hit = Physics2D.OverlapPoint(finalPos);
                                if (!hit)
                                {
                                    quadTree.Insert(finalPos);
                                }
                            }
                            break;
                        }
                    } 
                }
                
            }
        }
        
    }

    public List<Vector3> QueryQuadTree(Bounds boundry)
    {

        return quadTree.QueryTree(boundry);
    }
}
