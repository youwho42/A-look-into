using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMessengerArea : MonoBehaviour
{
    public QI_ItemData messageItem;
    public TalkBallMessageType messageType;
    public UndertakingObject undertaking;
    [HideInInspector]
    public bool hasSpawned;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawned)
            return;
        if (collision.CompareTag("Player"))
        {
            TalkBallManager.instance.SpawnMessenger(messageItem, messageType, undertaking);
            hasSpawned = true;
        }
        
        
    }
}
