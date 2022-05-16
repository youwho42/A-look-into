using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : Interactable
{
    public GameObject fireAnimation;
    public LightFlicker lightFlicker;
    bool isLit;

    public override void Interact(GameObject interactor)
    {
        
        base.Interact(interactor);

        if (!isLit)
        {
            isLit = true;
            fireAnimation.SetActive(true);
            
            interactVerb = "extinguish";
        }
        else
        {
            fireAnimation.SetActive(false);
            interactVerb = "light";
            isLit = false;
        }

    }

 

}
