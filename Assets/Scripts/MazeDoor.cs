using Klaxon.Interactable;
using Klaxon.MazeTech;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDoor : Interactable
{
    public GameObject doorOpen;
    public GameObject doorClosed;
    public bool isOpen;
    public QI_ItemData mazeTicketItem;
    public MazeCreator mazeCreator;
    public override void Start()
    {
        base.Start();

        doorOpen.SetActive(false);
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        InteractWithDoor();
    }

    public void InteractWithDoor()
    {
        if(!isOpen && mazeCreator.mazeSet)
        {
            if (!PlayerInformation.instance.playerInventory.HasItem(mazeTicketItem, 1))
                return;
            PlayerInformation.instance.playerInventory.RemoveItem(mazeTicketItem, 1);
            
            canInteract = false;
            isOpen = true;
            doorOpen.SetActive(true);
            doorClosed.SetActive(false);
        }
    }

    public void ResetDoor()
    {
        canInteract = true;
        hasInteracted = false;
        doorOpen.SetActive(false);
        doorClosed.SetActive(true);
        isOpen = false;
    }

    public void SetDoorInMaze(bool closed)
    {
        canInteract = !closed;
        hasInteracted = closed;
        doorOpen.SetActive(!closed);
        doorClosed.SetActive(closed);
        isOpen = !closed;
    }
    public void SetDoorFromSave(bool doorIsOpen)
    {
        canInteract = !doorIsOpen;
        isOpen = doorIsOpen;
        doorOpen.SetActive(isOpen);
        doorClosed.SetActive(!isOpen);
    }
}
