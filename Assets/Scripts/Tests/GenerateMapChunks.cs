using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class MapChunk
{
    public BoundsInt chunkArea;
    public TileBase[] tileArray;
    public Vector2 centerTilePosition;
    public bool isVisible;

    public MapChunk(BoundsInt area, TileBase[] tiles, Vector2 position)
    {
        chunkArea = area;
        tileArray = tiles;
        centerTilePosition = position;
    }
}


public class GenerateMapChunks : MonoBehaviour
{
    int chunkSize = 10;

    public Transform other;

    public float chunkDistance;

    TileBase[] emptyTileArray;

    public Tilemap tilemap;

    BoundsInt fullMap;

    
    public List<MapChunk> allMapChunks = new List<MapChunk>();

    /*private void Start()
    {
        
        GenerateChunks();

        SetEmptyTileArray();

        HideAllChunks();
    }*/

    void SetEmptyTileArray()
    {
        fullMap = tilemap.cellBounds;
        emptyTileArray = new TileBase[chunkSize * chunkSize * chunkSize];
        for (int index = 0; index < emptyTileArray.Length; index++)
        {
            emptyTileArray[index] = null;
        }
    }

    private void Update()
    {
        for (int i = 0; i < allMapChunks.Count; i++)
        {
            var dist = Vector2.Distance(other.position, allMapChunks[i].centerTilePosition);

            if (dist <= chunkDistance)
            {
                if (!allMapChunks[i].isVisible)
                    DrawChunk(allMapChunks[i]);
            }
            else
            {
                if (allMapChunks[i].isVisible)
                    HideChunk(allMapChunks[i]);
            }

        }

    }

    public void GenerateChunks()
    {
        allMapChunks.Clear();
        fullMap = tilemap.cellBounds;
        int chunksX = Mathf.CeilToInt(fullMap.size.x / (float)chunkSize);
        float chunksY = Mathf.CeilToInt(fullMap.size.y / (float)chunkSize);
        
        for (int x = 0; x < chunksX; x++)
        {
            for (int y = 0; y < chunksY; y++)
            {
                BoundsInt newArea = new BoundsInt(new Vector3Int(fullMap.xMin + chunkSize * x, fullMap.yMin + chunkSize * y, fullMap.zMin), new Vector3Int(chunkSize, chunkSize, chunkSize));
                TileBase[] tileArray = new TileBase[newArea.size.x * newArea.size.y * newArea.size.z];
                tileArray = tilemap.GetTilesBlock(newArea);
                var pos = tilemap.CellToWorld(new Vector3Int(newArea.position.x + chunkSize/2, newArea.position.y + chunkSize / 2, 0));
                MapChunk newChunk = new MapChunk(newArea, tileArray, pos);
                allMapChunks.Add(newChunk);
                
            }
        }
    }

   
    

    public void HideAllChunks()
    {
        
        foreach (var chunk in allMapChunks)
        {
            HideChunk(chunk);
        }
    }
    public void DrawAllChunks()
    {
        foreach (var chunk in allMapChunks)
        {
            DrawChunk(chunk);
        }
    }

    public void HideChunk(MapChunk chunk)
    {
        
        SetEmptyTileArray();

        chunk.isVisible = false;
        tilemap.SetTilesBlock(chunk.chunkArea, emptyTileArray);
        tilemap.CompressBounds();
    }

    public void DrawChunk(MapChunk chunk)
    {
        
        chunk.isVisible = true;
        tilemap.SetTilesBlock(chunk.chunkArea, chunk.tileArray);
        tilemap.CompressBounds();
    }


    


}
