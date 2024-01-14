using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallPersonVillager : MonoBehaviour
{
    public bool canLightFires;
    private void Start()
    {
        BallPeopleManager.instance.SpawnVillager(gameObject, transform.position, canLightFires);
    }

}
