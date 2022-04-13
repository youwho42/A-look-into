using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public RuleTile tile;
    public Tilemap tilemap;
    public int mapSize;
   
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;


    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public int mapMinZ;
    public Vector2Int mapOffset;
    BoundsInt area;

    public Texture2D mapImage;

    public void GenerateMapFromNoise()
    {
        //tilemap.ClearAllTiles();
        ClearTilesBlock();
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapSize, mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        int index = 0;
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {

                int currentZ = Mathf.RoundToInt(noiseMap[x, y] * 10);
                
                for (int z = 0; z < 10; z++)
                {
                    if(z <= currentZ)
                    {
                        tileArray[index] = tile;
                    }
                    index++; 
                    
                }
                
            }
        }
        tilemap.SetTilesBlock(area, tileArray);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
        tilemap.CompressBounds();
       
    }

    public void GenerateMapFromImage()
    {
        if (mapImage == null)
            return;
        ClearTilesBlock();

        float[,] imageMap = ConvertImage();
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        int index = 0;
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {

                int currentZ = Mathf.RoundToInt(imageMap[x, y] * 10);

                for (int z = 0; z < 10; z++)
                {
                    if (z <= currentZ)
                    {
                        tileArray[index] = tile;
                    }
                    index++;

                }

            }
        }
        tilemap.SetTilesBlock(area, tileArray);
        
        tilemap.CompressBounds();

    }

    float[,] ConvertImage() 
    {
        
        float[,] convertedImage = new float[mapImage.width, mapImage.height];
        for (int x = 0; x < mapImage.width; x++)
        {
            for (int y = 0; y < mapImage.height; y++)
            {
                convertedImage[x, y] = mapImage.GetPixel(x, y).grayscale;
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
       
        tilemap.SetTilesBlock(area, tileArray);
        tilemap.CompressBounds();
    }
    
    

    private void OnValidate()
    {
        if (mapImage != null)
            mapSize = mapImage.width;
        
        if (mapSize < 1)
            mapSize = 1;

        area = new BoundsInt(new Vector3Int(mapOffset.x, mapOffset.y, mapMinZ), new Vector3Int(mapSize, mapSize, 10));

        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 1)
            octaves = 1;
    }
}
