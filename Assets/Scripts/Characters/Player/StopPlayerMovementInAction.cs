using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPlayerMovementInAction : MonoBehaviour
{
    GravityItemMovementControllerNew playerMovement;

    void Start()
    {
        playerMovement = GetComponentInParent<GravityItemMovementControllerNew>();
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
