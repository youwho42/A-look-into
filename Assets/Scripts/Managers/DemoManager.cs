using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DemoManager : MonoBehaviour
{
    public static DemoManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [SerializeField]
    bool isDemo;
    [SerializeField]
    bool isPlaytest;
    public Transform startArea;
    PlayerInformation playerInformation;
    //public Tilemap tileMap;

    //public Vector3Int demoBounds;

    public Texture2D demoAreaTexture;
    [HideInInspector]
    public Color[,] demoArea;
    
    GridManager gridManager;



    public void SetColorArray()
    {
        demoArea = NumberFunctions.ConvertImage(demoAreaTexture);
    }

    private void Start()
    {
        gridManager = GridManager.instance;
        playerInformation = PlayerInformation.instance;
        if (isDemo)
            GameEventManager.onPlayerPositionUpdateEvent.AddListener(CheckPlayerPosition);
        SetColorArray();
    }

    private void OnDisable()
    {
        if (isDemo) 
            GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(CheckPlayerPosition);

    }

    public bool IsDemoVersion()
    {
        return isDemo;
    }

    public bool IsPlaytestVersion()
    {
        return isPlaytest;
    }

    void CheckPlayerPosition(Vector3Int pPos)
    {
        if (!isDemo)
            return;
        if(playerInformation == null)
            playerInformation = PlayerInformation.instance;



        int mapPositionX = (int)NumberFunctions.RemapNumber(pPos.x, gridManager.groundMap.cellBounds.min.x, gridManager.groundMap.cellBounds.max.x, 0, 128);
        int mapPositionY = (int)NumberFunctions.RemapNumber(pPos.y, gridManager.groundMap.cellBounds.min.y, gridManager.groundMap.cellBounds.max.y, 0, 128);
        
        bool inArea = demoArea[mapPositionX, mapPositionY].a > 0.4f;

        if (!inArea)
        {
            playerInformation.playerTransform.position = startArea.position;
            playerInformation.currentTilePosition.position = playerInformation.currentTilePosition.GetCurrentTilePosition(startArea.position);
            playerInformation.playerController.currentLevel = (int)startArea.position.z - 1;
            Notifications.instance.SetNewNotification("Demo Bounds Breached!", null, 0, NotificationsType.Warning);
        }






        //Vector3Int pPos = playerInformation.currentTilePosition.position;
        //if (pPos.x > demoBounds.x || pPos.y < demoBounds.y)
        //{
        //    playerInformation.playerTransform.position = startArea.position;
        //    playerInformation.currentTilePosition.position = playerInformation.currentTilePosition.GetCurrentTilePosition(startArea.position);
        //    playerInformation.playerController.currentLevel = (int)startArea.position.z - 1;
        //    Notifications.instance.SetNewNotification("Demo Bounds Breached!", null, 0, NotificationsType.Warning);
        //}
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (tileMap == null)
    //        return;
    //    Gizmos.color = Color.magenta;
    //    Gizmos.DrawWireSphere(tileMap.GetCellCenterWorld(demoBounds), 0.2f);
    //}

}
