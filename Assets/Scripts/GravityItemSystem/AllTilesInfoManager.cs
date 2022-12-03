using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllTilesInfoManager : MonoBehaviour
{
    public static AllTilesInfoManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public TileInfoObject allTilesObject;
    public Dictionary<Vector3Int, List<TileDirectionInfo>> allTilesDictionary = new Dictionary<Vector3Int, List<TileDirectionInfo>>();

    private void Start()
    {
        for (int i = 0; i < allTilesObject.allDirectionsValues.Count; i++)
        {
            allTilesDictionary.Add(allTilesObject.allDirectionsValues[i].tilePosition, allTilesObject.allDirectionsValues[i].allTilesValues);
            
        }
        
    }
}
