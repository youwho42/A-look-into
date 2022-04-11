using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public RuleTile tile;
    public Tilemap tilemap;
    public int mapWitdh;
    public int mapHeight;
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
    public void GenerateMap()
    {
        //tilemap.ClearAllTiles();
        ClearTilesBlock();
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapWitdh, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        int index = 0;
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWitdh; x++)
            {

                int currentZ = Mathf.RoundToInt(noiseMap[x, y] * 10);
                
                for (int z = 0; z < 10; z++)
                {
                    if(z <= currentZ)
                    {
                        tileArray[index] = tile;
                        //tilemap.SetTile(new Vector3Int(x + mapOffset.x, y + mapOffset.y, z + mapMinZ), tile);
                    }
                    index++; 
                    
                }
                
            }
        }
        tilemap.SetTilesBlock(area, tileArray);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);

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
    /*void GenerateTileMap(float[,] map)
    {
        int[,] tilemapMap = new int[]
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWitdh; x++)
            {

            }
        }
    }*/
    

    private void OnValidate()
    {
        area = new BoundsInt(new Vector3Int(mapOffset.x, mapOffset.y, mapMinZ), new Vector3Int(mapWitdh, mapHeight, 10));
        if (mapWitdh < 1)
            mapWitdh = 1;
        if (mapHeight < 1)
            mapHeight = 1;
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 1)
            octaves = 1;
    }
}
