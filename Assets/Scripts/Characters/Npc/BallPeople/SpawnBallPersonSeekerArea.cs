using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallPersonSeekerArea : SpawnableBallPersonArea
{
    public QI_ItemData seekItem;
    public int seekAmount;
    public CompleteTaskObject talkTask;
    public CompleteTaskObject seekTask;


    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawned)
            return;
        if (collision.CompareTag("Player"))
        {
            BallPeopleManager.instance.SpawnSeeker(seekItem, seekAmount, talkTask, seekTask);
            hasSpawned = true;
        }


    }
}
