using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfoRequestManager : MonoBehaviour
{

    public static TileInfoRequestManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        allTilesInfo = GetComponent<AllTilesInfoManager>();
    }


    Queue<TileInfoRequest> tileInfoRequestQueue = new Queue<TileInfoRequest>();
    TileInfoRequest currentTileInfoRequest;

    AllTilesInfoManager allTilesInfo;
    bool isProcessingTileInfo;
    //static List<IsometricNode> walkableNodes = new List<IsometricNode>();
    bool tileInfoSuccess;

    struct TileInfoRequest
    {
        public Vector3Int tileInfoPosition;
        public Action<List<TileDirectionInfo>, bool> tileInfoCallback;

        public TileInfoRequest(Vector3Int position, Action<List<TileDirectionInfo>, bool> callback)
        {
            tileInfoPosition = position;
            tileInfoCallback = callback;
        }
    }



    public static void RequestTileInfo(Vector3Int tileInfoPosition, Action<List<TileDirectionInfo>, bool> callback)
    {
        TileInfoRequest newRequest = new TileInfoRequest(tileInfoPosition, callback);
        instance.tileInfoRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingTileInfo && tileInfoRequestQueue.Count > 0)
        {
            currentTileInfoRequest = tileInfoRequestQueue.Dequeue();
            isProcessingTileInfo = true;
            GetTileInfo(currentTileInfoRequest.tileInfoPosition);
        }
    }

    
    void GetTileInfo(Vector3Int startPos)
    {
        
        tileInfoSuccess = false;

        //get tile info
        List<TileDirectionInfo> tileBlock;
        allTilesInfo.allTilesDictionary.TryGetValue(currentTileInfoRequest.tileInfoPosition, out tileBlock);


        if (tileBlock!=null)
        {
            tileInfoSuccess = true;
        }
        FinishedProcessingPath(tileBlock, tileInfoSuccess);
    }


    public void FinishedProcessingPath(List<TileDirectionInfo> tileInfoBlock, bool success)
    {
        currentTileInfoRequest.tileInfoCallback(tileInfoBlock, success);
        isProcessingTileInfo = false;
        TryProcessNext();
    }

}
