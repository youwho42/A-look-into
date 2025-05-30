using Klaxon.Interactable;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.Localization;

public class SpawnBallPersonLocationerArea : SpawnableBallPersonArea
{
    
    public UndertakingObject undertaking;
    public Transform locationerLocation;
    public LocalizedString messageTitle;
    public LocalizedString messageDescription;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawned)
            return;
        if (collision.CompareTag("Player"))
        {
            if (collision.gameObject.transform.position.z == transform.position.z)
            {
                BallPeopleManager.instance.SpawnLocationer(undertaking, locationerLocation, marker.transform.position, messageTitle, messageDescription);
                marker.enabled = false;
                hasSpawned = true;
            }
        }


    }
}
