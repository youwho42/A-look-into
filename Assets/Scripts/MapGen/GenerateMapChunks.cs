using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class MapChunk
{
    public BoundsInt chunkArea;
    public TileBase[] groundTileArray;
    public TileBase[] waterTileArray;
    public TileBase[] groundDecoOneTileArray;
    public TileBase[] groundDecoTwoTileArray;
    public Vector2 centerTilePosition;
    public bool isVisible;
    
    
    public MapChunk(BoundsInt area, TileBase[] groundTiles, TileBase[] waterTiles, TileBase[] gdOneTileArray, TileBase[] gdTwoTileArray, Vector2 position)
    {
        chunkArea = area;
        groundTileArray = groundTiles;
        waterTileArray = waterTiles;
        centerTilePosition = position;
        groundDecoOneTileArray = gdOneTileArray;
        groundDecoTwoTileArray = gdTwoTileArray;
    }
}


public class GenerateMapChunks : MonoBehaviour
{
    int chunkSize = 10;

    TileBase[] emptyTileArray;

    public Tilemap groundTilemap;
    public Tilemap waterTilemap;
    public Tilemap gdOneTilemap;
    public Tilemap gdTwoTilemap;

    [HideInInspector]
    public BoundsInt fullMap;
    public string mapName;
    
    [HideInInspector]
    public List<MapChunk> allMapChunks = new List<MapChunk>();
    [SerializeField]
    public ChunkTerrainData chunkLoadScriptableObject;
    


    private void Start()
    {
        DrawAllChunks();
    }
    void SetEmptyTileArray(int size)
    {
        fullMap = groundTilemap.cellBounds;
        emptyTileArray = new TileBase[size];
        for (int index = 0; index < emptyTileArray.Length; index++)
        {
            emptyTileArray[index] = null;
        }
    }


  
    public void LoadChunksFromSave()
    {
        
        if (chunkLoadScriptableObject == null)
            return;

        allMapChunks = new List<MapChunk>(chunkLoadScriptableObject.allChunks);
       
       
    }



    public void GenerateChunks()
    {
        if(mapName == "")
        {
            throw new Exception("You need to name the map doofus");
        }

        
        fullMap = groundTilemap.cellBounds;
        int chunksX = Mathf.CeilToInt(fullMap.size.x / (float)chunkSize);
        int chunksY = Mathf.CeilToInt(fullMap.size.y / (float)chunkSize);
        allMapChunks.Clear();
        for (int x = 0; x < chunksX; x++)
        {
            for (int y = 0; y < chunksY; y++)
            {
                BoundsInt newArea = new BoundsInt(new Vector3Int(fullMap.xMin + chunkSize * x, fullMap.yMin + chunkSize * y, fullMap.zMin), new Vector3Int(chunkSize, chunkSize, fullMap.size.z));
                TileBase[] gTileArray = new TileBase[newArea.size.x * newArea.size.y * newArea.size.z];
                gTileArray = groundTilemap.GetTilesBlock(newArea);
                TileBase[] wTileArray = new TileBase[newArea.size.x * newArea.size.y * newArea.size.z];
                wTileArray = waterTilemap.GetTilesBlock(newArea);
                TileBase[] gdOneTileArray = new TileBase[newArea.size.x * newArea.size.y * newArea.size.z];
                gdOneTileArray = gdOneTilemap.GetTilesBlock(newArea);
                TileBase[] gdTwoTileArray = new TileBase[newArea.size.x * newArea.size.y * newArea.size.z];
                gdTwoTileArray = gdTwoTilemap.GetTilesBlock(newArea);

                if (CheckChunkEmpty(gTileArray) && CheckChunkEmpty(wTileArray))
                    continue;
                var pos = groundTilemap.CellToWorld(new Vector3Int(newArea.position.x + chunkSize/2, newArea.position.y + chunkSize / 2, 0));
                MapChunk newChunk = new MapChunk(newArea, gTileArray, wTileArray, gdOneTileArray, gdTwoTileArray, pos);
                allMapChunks.Add(newChunk);
                
                
            }
        }
        
    }

    

    bool CheckChunkEmpty(TileBase[] tileArray)
    {
        for (int i = 0; i < tileArray.Length; i++)
        {
            if (tileArray[i] != null)
                return false;
        }
        return true;
    }


    public void UpdateSelectedChunks(List<MapChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            UpdateChunk(chunk);
        }
    }
    void UpdateChunk(MapChunk chunk)
    {
        for (int i = 0; i < allMapChunks.Count; i++)
        {
            if(allMapChunks[i].centerTilePosition == chunk.centerTilePosition)
                allMapChunks[i] = new MapChunk(chunk.chunkArea, chunk.groundTileArray, chunk.waterTileArray, chunk.groundDecoOneTileArray, chunk.groundDecoTwoTileArray, chunk.centerTilePosition);
        }
        
        
    }



    public void DrawSelectedChunks(List<MapChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
           DrawChunk(chunk);
        }
    }
    public void HideSelectedChunks(List<MapChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            HideChunk(chunk);
        }
    }

    public void HideAllChunks()
    {
        
        foreach (var chunk in allMapChunks)
        {
            if (chunk.isVisible)
                HideChunk(chunk);
        }
        
    }
    public void DrawAllChunks()
    {
        foreach (var chunk in allMapChunks)
        {
            if(!chunk.isVisible)
                DrawChunk(chunk);
        }
        
    }

    public void HideChunk(MapChunk chunk)
    {
        
        SetEmptyTileArray(chunk.chunkArea.size.x * chunk.chunkArea.size.y * chunk.chunkArea.size.z);

        chunk.isVisible = false;
        groundTilemap.SetTilesBlock(chunk.chunkArea, emptyTileArray);
        groundTilemap.CompressBounds();
        waterTilemap.SetTilesBlock(chunk.chunkArea, emptyTileArray);
        waterTilemap.CompressBounds();
        gdOneTilemap.SetTilesBlock(chunk.chunkArea, emptyTileArray);
        gdOneTilemap.CompressBounds();
        gdTwoTilemap.SetTilesBlock(chunk.chunkArea, emptyTileArray);
        gdTwoTilemap.CompressBounds();
    }

    public void DrawChunk(MapChunk chunk)
    {
        
        chunk.isVisible = true;
        groundTilemap.SetTilesBlock(chunk.chunkArea, chunk.groundTileArray);
        groundTilemap.CompressBounds();
        waterTilemap.SetTilesBlock(chunk.chunkArea, chunk.waterTileArray);
        waterTilemap.CompressBounds();
        
        gdOneTilemap.SetTilesBlock(chunk.chunkArea, chunk.groundDecoOneTileArray);
        gdOneTilemap.CompressBounds();
        
       
        gdTwoTilemap.SetTilesBlock(chunk.chunkArea, chunk.groundDecoTwoTileArray);
        gdTwoTilemap.CompressBounds();
        
    }


    


}
