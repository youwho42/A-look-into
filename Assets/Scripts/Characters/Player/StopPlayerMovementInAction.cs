using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPlayerMovementInAction : MonoBehaviour
{
    Playermovement playerMovement;

    void Start()
    {
        playerMovement = GetComponentInParent<Playermovement>();
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
