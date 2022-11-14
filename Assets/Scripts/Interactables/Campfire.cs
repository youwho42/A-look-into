using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Campfire : Interactable
{
    public GameObject fireAnimation;
    public Light2D lightFlicker;
    bool isLit;

    public override void Interact(GameObject interactor)
    {

        base.Interact(interactor);

        if (!isLit)
        {
            isLit = true;
            fireAnimation.SetActive(true);
            lightFlicker.enabled = true;
           
            interactVerb = "extinguish";
        }
        else
        {
            fireAnimation.SetActive(false);
            lightFlicker.enabled = false;
            interactVerb = "light";
            isLit = false;
        }

    }

    
}
