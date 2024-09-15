using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallPersonVillager : MonoBehaviour
{
    public CompleteTaskObject taskObject;
    private void Start()
    {
        BallPeopleManager.instance.SpawnVillager(taskObject, transform.position);
    }

}
