using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallPersonMessengerArea : SpawnableBallPersonArea
{
    public QI_ItemData messageItem;
    public BallPeopleMessageType messageType;
    public UndertakingObject undertaking;

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawned)
            return;
        if (collision.CompareTag("Player"))
        {
            BallPeopleManager.instance.SpawnMessenger(messageItem, messageType, undertaking);
            hasSpawned = true;
        }
        
        
    }
}
