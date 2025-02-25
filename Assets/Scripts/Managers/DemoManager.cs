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
    public Tilemap tileMap;

    public Vector3Int demoBounds;

    private void Start()
    {
        playerInformation = PlayerInformation.instance;
        if (isDemo)
            GameEventManager.onPlayerPositionUpdateEvent.AddListener(CheckPlayerPosition);
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

    void CheckPlayerPosition()
    {
        if (!isDemo)
            return;
        if(playerInformation == null)
            playerInformation = PlayerInformation.instance;
        Vector3Int pPos = playerInformation.currentTilePosition.position;
        if(pPos.x > demoBounds.x || pPos.y < demoBounds.y)
        {
            PlayerInformation.instance.player.position = startArea.position;
            PlayerInformation.instance.currentTilePosition.position = PlayerInformation.instance.currentTilePosition.GetCurrentTilePosition(startArea.position);
            PlayerInformation.instance.playerController.currentLevel = (int)startArea.position.z - 1;
            Notifications.instance.SetNewNotification("Demo Bounds Breached!", null, 0, NotificationsType.Warning);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(tileMap.GetCellCenterWorld(demoBounds), 0.2f);
    }

}
