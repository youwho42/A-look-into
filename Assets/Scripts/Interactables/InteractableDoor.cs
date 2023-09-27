using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : Interactable
{

    
    public bool isOpen;
    public float maxOpenTime = 1;
    public GameObject doorOpen_Upper;
    public GameObject doorClosed;
    public GameObject doorOpen_Lower;
    

    public override void Start()
    {
        base.Start();
        
        doorOpen_Lower.SetActive(false);
        doorOpen_Upper.SetActive(false);
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        InteractWithDoor(interactor);
    }

    void InteractWithDoor(GameObject interactor)
    {
        GameObject openState = interactor.transform.position.y > transform.position.y ? doorOpen_Lower : doorOpen_Upper;
        GameObject otherState = interactor.transform.position.y < transform.position.y ? doorOpen_Lower : doorOpen_Upper;
        PlayInteractionSound();
        isOpen = !isOpen;
        if (isOpen)
        {
            interactVerb = "Close";
            openState.SetActive(true);
            otherState.SetActive(false);
            doorClosed.SetActive(false);
            Invoke("CloseDoor", maxOpenTime);
        }
        else
        {
            CloseDoor();
        }
    }
    void CloseDoor()
    {
        isOpen = false;
        interactVerb = "Open";
        doorOpen_Lower.SetActive(false);
        doorOpen_Upper.SetActive(false);
        doorClosed.SetActive(true);
    }
    public virtual void PlayInteractionSound()
    {
        audioManager.PlaySound(interactSound);
    }
}
