using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallPersonTravellerArea : SpawnableBallPersonArea
{
    public CompleteTaskObject taskObject;
    public GameObject travellerDestination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawned)
            return;
        if (collision.CompareTag("Player"))
        {
            if (collision.gameObject.transform.position.z == transform.position.z)
            {
                BallPeopleManager.instance.SpawnTraveller(taskObject, travellerDestination, marker.transform.position);
                marker.enabled = false;
                hasSpawned = true;
            }
        }


    }
}
