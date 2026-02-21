using UnityEngine;
using System.Collections.Generic;


public class SpadeAreaInteractor : SpadeInteractable
{
    


    public override void EndSpadeInteraction()
    {
        base.EndSpadeInteraction();
        Destroy(gameObject);
    }

}
