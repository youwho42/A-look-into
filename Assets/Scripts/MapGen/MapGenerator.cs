using System;
using UnityEngine;
using UnityEngine.Tilemaps;


[Serializable]
public class WorldMap
{
    public BoundsInt chunkArea;
    public TileBase[] tileArray;
   

    public WorldMap(BoundsInt area, TileBase[] tiles)
    {
        chunkArea = area;
        tileArray = tiles;
        
    }
}

public class MapGenerator : MonoBehaviour
{
    public RuleTile gColorTile;
    public RuleTile rColorTile;
    public RuleTile bColorTile;
    public RuleTile snowColorTile;
    public Tile mud;
    public Tilemap groundTilemap;
    public Tilemap waterTilemap;
    [Space]
    public int mapSize;
    public int mapMaxSizeZ;
    public int mapMinZ;


    [Header("Noise")]
    public bool useOpenSimplex;
    public bool autoUpdate;
    public string mapName;

    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;


    public int seed;
    public Vector2 offset;

    

    [Space]
    public Vector2Int mapOffset;
    BoundsInt area;

    public Texture2D mapImage;
    
    public void GenerateFromSimplex()
    {

    }

    public void GenerateMapFromNoise()
    {
        ClearTilesBlock();
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(useOpenSimplex, mapSize, mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        GenerateMap(noiseMap);
        
        //MapDisplay display = FindObjectOfType<MapDisplay>();
        //display.DrawNoiseMap(noiseMap);
        
    }

    public void GenerateMapFromImage()
    {
        if (mapImage == null)
            return;

        ClearTilesBlock();
        Color[,] imageMap = ConvertImage();
        GenerateMap(imageMap);
        
        
    }

   

    void GenerateMap(Color[,] map)
    {
        TileBase[] groundTileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        TileBase[] waterTileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        int index = 0;
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {

                int currentLandZ = Mathf.RoundToInt(map[x, y].g * mapMaxSizeZ);
                

                for (int z = 0; z < mapMaxSizeZ; z++)
                {
                    if(z == Mathf.Abs(mapMinZ) && currentLandZ < Mathf.Abs(mapMinZ))
                    {
                        waterTileArray[index] = bColorTile;
                    }

                    if (z <= currentLandZ)
                    {
                        if (currentLandZ == 0)
                            groundTileArray[index] = null;
                        else if (currentLandZ > 0 && currentLandZ <= Mathf.Abs(mapMinZ)-1)
                            groundTileArray[index] = mud;
                        else if (currentLandZ > 0 && currentLandZ >= 15)
                            groundTileArray[index] = snowColorTile;
                        else
                        {
                            if (map[x, y].r == 1)
                            {
                                groundTileArray[index] = rColorTile;
                            }
                            else if (map[x, y].b == 1)
                            {
                                groundTileArray[index] = bColorTile;
                            }
                            else
                                groundTileArray[index] = gColorTile;
                            
                        }
                            
                    }
                    index++; 

                }

            }
        }
        
        groundTilemap.SetTilesBlock(area, groundTileArray);
        groundTilemap.CompressBounds();
        waterTilemap.SetTilesBlock(area, waterTileArray);
        waterTilemap.CompressBounds();
    }

    void GenerateMap(float[,] map)
    {
        TileBase[] groundTileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        TileBase[] waterTileArray = new TileBase[area.size.x * area.size.y * area.size.z];

        int index = 0;
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {

                int currentZ = Mathf.RoundToInt(map[x, y] * mapMaxSizeZ);

                for (int z = 0; z < mapMaxSizeZ; z++)
                {
                    if (z == Mathf.Abs(mapMinZ) && currentZ <= Mathf.Abs(mapMinZ) - 1)
                        waterTileArray[index] = bColorTile;

                    if (z <= currentZ)
                    {
                        if (currentZ == 0)
                            groundTileArray[index] = null;
                        else if (currentZ > 0 && currentZ <= Mathf.Abs(mapMinZ) - 1)
                            groundTileArray[index] = mud;
                        else if (currentZ > 0 && currentZ >= Mathf.Abs(15))
                            groundTileArray[index] = snowColorTile;
                        else
                            groundTileArray[index] = gColorTile;

                        
                    }
                    index++;

                }

            }
        }

        groundTilemap.SetTilesBlock(area, groundTileArray);
        groundTilemap.CompressBounds();
        waterTilemap.SetTilesBlock(area, waterTileArray);
        waterTilemap.CompressBounds();
    }


    Color[,] ConvertImage() 
    {
        
        Color[,] convertedImage = new Color[mapImage.width, mapImage.height];
        for (int x = 0; x < mapImage.width; x++)
        {
            for (int y = 0; y < mapImage.height; y++)
            {
                convertedImage[x, y] = mapImage.GetPixel(x, y);
            }
        }
        return convertedImage;
    }

    public void ClearTilesBlock()
    {
        
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        for (int index = 0; index < tileArray.Length; index++)
        {
            tileArray[index] = null;
        }
       
        groundTilemap.SetTilesBlock(area, tileArray);
        groundTilemap.CompressBounds();
        waterTilemap.SetTilesBlock(area, tileArray);
        waterTilemap.CompressBounds();
    }
    
    

    private void OnValidate()
    {
        if (mapImage != null)
            mapSize = mapImage.width;
        
        if (mapSize < 1)
            mapSize = 1;

        area = new BoundsInt(new Vector3Int(mapOffset.x, mapOffset.y, mapMinZ), new Vector3Int(mapSize, mapSize, mapMaxSizeZ));

        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 1)
            octaves = 1;
    }
}
