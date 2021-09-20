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
        playerMovement.isInAction = true;
    }
    public void StartPlayerInput()
    {
        playerMovement.isInAction = false;
    }
}
