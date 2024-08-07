﻿using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    
    [HideInInspector]
    public Material material;
    [HideInInspector]
    public GameObject objectToTeleport; 
    
    

    bool isInTeleportRange;
    [HideInInspector]
    public bool isIncoming;

    public int currentLevel;
    public string teleportName;

    private void Start()
    {
        TeleportSystemManager.instance.allTeleports.Add(transform);
        SetCurrentLevel();
    }

    public void StartTeleport(Transform teleporter)
    {
        StartCoroutine(TeleportCo(teleporter));
    }

    public void SetUpTeleporter(GameObject interactor)
    {
        material = interactor.GetComponentInChildren<SpriteRenderer>().material;
        objectToTeleport = interactor.gameObject;
        isInTeleportRange = true;
    }

    IEnumerator TeleportCo(Transform teleporter)
    {
        teleporter.GetComponent<Teleport>().isIncoming = true;
        Material usingMaterial = material;
        GameObject activeTeleport = objectToTeleport;
        activeTeleport.GetComponent<GravityItemMovementControllerNew>().enabled = false;

        DissolveEffect.instance.StartDissolve(usingMaterial, 2f, false);
        yield return new WaitForSeconds(2f);
        activeTeleport.SetActive(false);
        activeTeleport.transform.position = new Vector3(teleporter.position.x, teleporter.position.y, teleporter.GetComponent<Teleport>().currentLevel);
        
        yield return new WaitForSeconds(1f);

        activeTeleport.SetActive(true);



        DissolveEffect.instance.StartDissolve(usingMaterial, 2f, true);
        yield return new WaitForSeconds(2f);
        if (isIncoming && isInTeleportRange)
        {
            isIncoming = false;
        }
        material = null; ;
        objectToTeleport = null;
        isInTeleportRange = false;
        activeTeleport.GetComponent<GravityItemMovementControllerNew>().enabled = true;
    }


    public void SetCurrentLevel()
    {
        GetComponentInChildren<SpriteRenderer>().transform.position = new Vector3(transform.position.x, transform.position.y, currentLevel - 0.4f);
    }
  
}
