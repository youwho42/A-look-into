using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : Interactable
{

    Animator animator;
    public bool isOpen;
    PolygonCollider2D collider2D;

    private void Start()
    {
        animator = GetComponent<Animator>();
        collider2D = GetComponent<PolygonCollider2D>();
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        
        StartCoroutine("InteractWithDoor");
    }

    void InteractWithDoor()
    {
        animator.SetBool("IsOpen", !isOpen);
        
        isOpen = !isOpen;
        if (isOpen)
        {
            interactVerb = "Close";
        }
        else
        {
            interactVerb = "Open";
        }
    }
}
