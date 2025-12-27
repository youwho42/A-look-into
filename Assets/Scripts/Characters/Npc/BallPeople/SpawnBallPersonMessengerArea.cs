using Klaxon.Interactable;
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
    public QI_CraftingRecipe craftingRecipe;

    public override void SpawnBP()
    {
        if (hasSpawned)
            return;
       
        BallPeopleManager.instance.SpawnMessenger(messageItem, messageType, undertaking, craftingRecipe, marker.transform.position);
        marker.enabled = false;
        hasSpawned = true;
            
        
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (hasSpawned)
    //        return;
    //    if (collision.CompareTag("Player"))
    //    {
    //        if (collision.gameObject.transform.position.z == transform.position.z)
    //        {
    //            BallPeopleManager.instance.SpawnMessenger(messageItem, messageType, undertaking, craftingRecipe, marker.transform.position);
    //            marker.enabled = false;
    //            hasSpawned = true;
    //        }
    //    }
        
        
    //}
}
