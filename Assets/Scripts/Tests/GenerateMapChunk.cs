using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateMapChunk : MonoBehaviour
{
    int chunkSize = 10;
    public class MapChunk
    {
        public BoundsInt chunkArea;
        public TileBase[] tileArray;

        public MapChunk(BoundsInt area, TileBase[] tiles)
        {
            chunkArea = area;
            tileArray = tiles;
        }
    }
    public Tilemap tilemap;

    BoundsInt fullMap;

    private void Start()
    {
        fullMap = tilemap.cellBounds;

        int chunksX = Mathf.CeilToInt(fullMap.size.x / chunkSize);
        int chunksY = Mathf.CeilToInt(fullMap.size.y / chunkSize);
        for (int x = 0; x < chunksX; x++)
        {
            for (int y = 0; y < chunksY; y++)
            {

                BoundsInt newArea = new BoundsInt(new Vector3Int(fullMap.min.x * x, fullMap.min.y * y, fullMap.min.z), new Vector3Int(chunkSize, chunkSize, fullMap.size.z));

                TileBase[] tileArray = tilemap.GetTilesBlock(newArea);
                
                MapChunk newChunk = new MapChunk(newArea, tileArray);
            }
        }
    }

    public void GenerateChunk(BoundsInt area)
    {
        //tilemap.GetTilesBlock(area);
    }
}
