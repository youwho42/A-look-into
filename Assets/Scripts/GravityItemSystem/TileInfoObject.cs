using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileInfoObject : ScriptableObject
{
    
 
    public List<TileBlockInfo> allDirectionsValues = new List<TileBlockInfo>();


    public void SaveTileInfo(List<TileBlockInfo> allValues)
    {
        
        allDirectionsValues = new List<TileBlockInfo>(allValues);
        
    }
}
