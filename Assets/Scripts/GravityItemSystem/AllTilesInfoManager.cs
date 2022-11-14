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



}
