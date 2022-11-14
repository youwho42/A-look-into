using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DestinationButton : MonoBehaviour
{

    Transform destination;
    Teleport departingTeleport;
    [SerializeField]
    private TextMeshProUGUI destinationName;
    public void AddDestination(Transform newDestination, Teleport teleport)
    {
        destination = newDestination;
        departingTeleport = teleport;
        destinationName.text = teleport.teleportName;
    }


    public void Teleport()
    {
        departingTeleport.StartTeleport(destination);
        TeleportDisplayUI.instance.HideUI();
    }
}
