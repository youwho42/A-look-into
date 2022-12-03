using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActiveOnPlayerDistance : MonoBehaviour
{

    CurrentTilePosition[] allAnimals;
    PlayerInformation playerInfo;

    private void Start()
    {
        playerInfo = PlayerInformation.instance;
        allAnimals = GetComponentsInChildren<CurrentTilePosition>();
       
    }


    private void Update()
    {
        for (int i = 0; i < allAnimals.Length; i++)
        {
            var dist = Vector3Int.Distance(playerInfo.currentTilePosition.position, allAnimals[i].position);
            allAnimals[i].gameObject.SetActive(dist < 20);
        }
        
    }


}
