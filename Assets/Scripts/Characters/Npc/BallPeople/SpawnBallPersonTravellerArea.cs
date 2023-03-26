using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallPersonTravellerArea : MonoBehaviour
{
    public UndertakingObject undertaking;
    public GameObject travellerDestination;
    [HideInInspector]
    public bool hasSpawned;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawned)
            return;
        if (collision.CompareTag("Player"))
        {
            BallPeopleManager.instance.SpawnTraveller(undertaking, travellerDestination);
            hasSpawned = true;
        }


    }
}
