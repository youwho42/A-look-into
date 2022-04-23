using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChunkTerrainData : ScriptableObject
{

    public List<MapChunk> allChunks;
    public Vector2Int mapOffset;
    public void SaveTerrainData(List<MapChunk> chunks, Vector2Int offset)
    {
        allChunks = new List<MapChunk>(chunks);
        mapOffset = new Vector2Int(offset.x, offset.y);
    }
    public void UpdateChunks(MapChunk chunk)
    {
        for (int i = 0; i < allChunks.Count; i++)
        {
            if (allChunks[i].centerTilePosition == chunk.centerTilePosition)
                allChunks[i] = new MapChunk(chunk.chunkArea, chunk.groundTileArray, chunk.waterTileArray, chunk.groundDecoOneTileArray, chunk.groundDecoTwoTileArray, chunk.centerTilePosition);
        }
    }


}
