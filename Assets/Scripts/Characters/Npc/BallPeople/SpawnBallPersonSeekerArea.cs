using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallPersonSeekerArea : MonoBehaviour
{
    public QI_ItemData seekItem;
    public int seekAmount;
    //public UndertakingObject undertaking;
    [HideInInspector]
    public bool hasSpawned;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawned)
            return;
        if (collision.CompareTag("Player"))
        {
            BallPeopleManager.instance.SpawnSeeker(seekItem, seekAmount);
            hasSpawned = true;
        }


    }
}
