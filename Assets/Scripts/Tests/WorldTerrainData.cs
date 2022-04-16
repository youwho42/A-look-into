using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTerrainData : ScriptableObject
{
    public WorldMap map;

    public void SaveWorldTerrainData(WorldMap _map)
    {
        map = new WorldMap(_map.chunkArea, _map.tileArray);
    }
}
