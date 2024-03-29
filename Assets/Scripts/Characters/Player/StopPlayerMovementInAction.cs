using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;
public class StopPlayerMovementInAction : MonoBehaviour
{
    GravityItemMovementControllerNew playerMovement;

    void Start()
    {
        playerMovement = GetComponentInParent<GravityItemMovementControllerNew>();
    }

    public void StopPlayerInput()
    {
        if(playerMovement != null)
            playerMovement.isInInteractAction = true;
    }
    public void StartPlayerInput()
    {
        if (playerMovement != null)
            playerMovement.isInInteractAction = false;
    }
}
