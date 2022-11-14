using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTeleporter : Interactable
{
    Teleport teleport;
    bool isOpen;
    TeleportDisplayUI teleportUI;

    public override void Start()
    {
        base.Start();
        teleport = GetComponent<Teleport>();
        teleportUI = TeleportDisplayUI.instance;
    }
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        if (!isOpen)
        {
            teleportUI.ShowUI(teleport);
            isOpen = true;
        }
        else
        {
            teleportUI.HideUI();
            isOpen = false;
        }

        teleport.SetUpTeleporter(interactor);
    }
}
