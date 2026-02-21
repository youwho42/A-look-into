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



    public override void SpawnBP()
    {
        if (hasSpawned)
            return;
        
        BallPeopleManager.instance.SpawnSeeker(seekItem, seekAmount, talkTask, seekTask, marker.transform.position);
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
    //            BallPeopleManager.instance.SpawnSeeker(seekItem, seekAmount, talkTask, seekTask, marker.transform.position);
    //            marker.enabled = false;
    //            hasSpawned = true;
    //        }
    //    }


    //}
}
