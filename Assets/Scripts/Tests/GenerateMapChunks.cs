using System;
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
    
    public MapChunk(BoundsInt area, TileBase[] groundTiles, TileBase[] waterTiles, Vector2 position)
    {
        chunkArea = area;
        groundTileArray = groundTiles;
        waterTileArray = waterTiles;
        centerTilePosition = position;
        groundDecoOneTileArray = new TileBase[0];
        groundDecoTwoTileArray = new TileBase[0];
    }
}


public class GenerateMapChunks : MonoBehaviour
{
    int chunkSize = 10;

    public Transform other;

    public float chunkDistance;

    TileBase[] emptyTileArray;

    public Tilemap groundTilemap;
    public Tilemap waterTilemap;
    public Tilemap gdOneTilemap;
    public Tilemap gdTwoTilemap;

    BoundsInt fullMap;
    public string mapName;
    
    [HideInInspector]
    public List<MapChunk> allMapChunks = new List<MapChunk>();
    public ChunkTerrainData chunkLoadScriptableObject;


    private void Start()
    {
        HideAllChunks();
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
    

    public void SaveChunks()
    {
        if (allMapChunks.Count <= 0)
            return;
        ChunkTerrainData newMap = ScriptableObject.CreateInstance<ChunkTerrainData>();

        newMap.SaveTerrainData(allMapChunks);
        if (!AssetDatabase.IsValidFolder("Assets/Terrain/"))
            AssetDatabase.CreateFolder("Assets/", "Terrain");

        string path = "Assets/Terrain/" + mapName + ".asset";
        
        AssetDatabase.CreateAsset(newMap, path);
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newMap;
    }




    public void LoadChunksFromSave()
    {
        
        if (chunkLoadScriptableObject == null)
            return;

       
        allMapChunks = new List<MapChunk>(chunkLoadScriptableObject.allMapChunks);
       
    }



    public void GenerateChunks()
    {
        allMapChunks.Clear();
        fullMap = groundTilemap.cellBounds;
        int chunksX = Mathf.CeilToInt(fullMap.size.x / (float)chunkSize);
        float chunksY = Mathf.CeilToInt(fullMap.size.y / (float)chunkSize);
        
        for (int x = 0; x < chunksX; x++)
        {
            for (int y = 0; y < chunksY; y++)
            {
                BoundsInt newArea = new BoundsInt(new Vector3Int(fullMap.xMin + chunkSize * x, fullMap.yMin + chunkSize * y, fullMap.zMin), new Vector3Int(chunkSize, chunkSize, fullMap.size.z));
                TileBase[] gTileArray = new TileBase[newArea.size.x * newArea.size.y * newArea.size.z];
                gTileArray = groundTilemap.GetTilesBlock(newArea);
                TileBase[] wTileArray = new TileBase[newArea.size.x * newArea.size.y * newArea.size.z];
                wTileArray = waterTilemap.GetTilesBlock(newArea);

                if (CheckChunkEmpty(gTileArray) && CheckChunkEmpty(wTileArray))
                    continue;
                var pos = groundTilemap.CellToWorld(new Vector3Int(newArea.position.x + chunkSize/2, newArea.position.y + chunkSize / 2, 0));
                MapChunk newChunk = new MapChunk(newArea, gTileArray, wTileArray, pos);
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
        if(allMapChunks.Contains(chunk))
        {
            TileBase[] gTileArray = new TileBase[chunk.chunkArea.size.x * chunk.chunkArea.size.y * chunk.chunkArea.size.z];
            gTileArray = groundTilemap.GetTilesBlock(chunk.chunkArea);
            TileBase[] wTileArray = new TileBase[chunk.chunkArea.size.x * chunk.chunkArea.size.y * chunk.chunkArea.size.z];
            wTileArray = waterTilemap.GetTilesBlock(chunk.chunkArea);
            TileBase[] gdOneTileArray = new TileBase[chunk.chunkArea.size.x * chunk.chunkArea.size.y * chunk.chunkArea.size.z];
            gdOneTileArray = gdOneTilemap.GetTilesBlock(chunk.chunkArea);
            TileBase[] gdTwoTileArray = new TileBase[chunk.chunkArea.size.x * chunk.chunkArea.size.y * chunk.chunkArea.size.z];
            gdTwoTileArray = gdTwoTilemap.GetTilesBlock(chunk.chunkArea);

            chunk.groundTileArray = gTileArray;
            chunk.waterTileArray = wTileArray;
            chunk.groundDecoOneTileArray = gdOneTileArray;
            chunk.groundDecoTwoTileArray = gdTwoTileArray;
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
        if(chunk.groundDecoOneTileArray.Length > 0)
        {
            gdOneTilemap.SetTilesBlock(chunk.chunkArea, chunk.groundDecoOneTileArray);
            gdOneTilemap.CompressBounds();
        }
        if (chunk.groundDecoTwoTileArray.Length > 0)
        {
            gdTwoTilemap.SetTilesBlock(chunk.chunkArea, chunk.groundDecoTwoTileArray);
            gdTwoTilemap.CompressBounds();
        }
    }


    


}
