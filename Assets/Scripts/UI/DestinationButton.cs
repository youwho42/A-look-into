using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationButton : MonoBehaviour
{

    Transform destination;
    Teleport departingTeleport;
    public void AddDestination(Transform newDestination, Teleport teleport)
    {
        destination = newDestination;
        departingTeleport = teleport;
    }


    public void Teleport()
    {
        departingTeleport.StartTeleport(destination);
        TeleportDisplayUI.instance.HideUI();
    }
}
