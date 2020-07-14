using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : Interactable
{
    public GameObject fireAnimation;

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        fireAnimation.SetActive(true);
        canInteract = false;
    }

    


}
