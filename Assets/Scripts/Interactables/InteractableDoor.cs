using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : Interactable
{

    Animator animator;
    public bool isOpen;
    public PolygonCollider2D doorClosed;
    public PolygonCollider2D doorOpen;

    public override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        
        StartCoroutine("InteractWithDoor");
    }

    void InteractWithDoor()
    {
        animator.SetBool("IsOpen", !isOpen);
        PlayInteractionSound();
        isOpen = !isOpen;
        if (isOpen)
        {
            interactVerb = "Close";
            doorOpen.enabled = true;
            doorClosed.enabled = false;
            
        }
        else
        {
            interactVerb = "Open";
            doorOpen.enabled = false;
            doorClosed.enabled = true;
        }
    }
    public virtual void PlayInteractionSound()
    {
        audioManager.PlaySound(interactSound);
    }
}
