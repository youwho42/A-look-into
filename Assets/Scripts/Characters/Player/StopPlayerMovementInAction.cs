using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPlayerMovementInAction : MonoBehaviour
{
    GravityItemMovementController playerMovement;

    void Start()
    {
        playerMovement = GetComponentInParent<GravityItemMovementController>();
    }

    public void StopPlayerInput()
    {
        playerMovement.isInInteractAction = true;
    }
    public void StartPlayerInput()
    {
        playerMovement.isInInteractAction = false;
    }
}
