using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTerrainData : ScriptableObject
{

    public List<MapChunk> allMapChunks;

    public void SaveTerrainData(List<MapChunk> map)
    {
        allMapChunks = new List<MapChunk>(map);
        
    }


}
