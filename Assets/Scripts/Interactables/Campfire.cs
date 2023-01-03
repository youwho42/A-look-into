using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Campfire : Interactable
{
    public GameObject fireAnimation;
    public Light2D lightFlicker;
    public FireFlicker fireFlicker;
    bool isLit;

    public override void Start()
    {
        base.Start();
        SetFire("light", false);
    }
    public override void Interact(GameObject interactor)
    {

        base.Interact(interactor);

        if (!isLit)
            SetFire("extinguish", true);
        else
            SetFire("light", false);
            
    }

    void SetFire(string _interactVerb, bool active)
    {
        isLit = active;
        fireAnimation.SetActive(active);
        lightFlicker.enabled = active;
        fireFlicker.canFlicker = active;
        interactVerb = _interactVerb;
        fireFlicker.StartLightFlicker(active);
        
    }

    
}
