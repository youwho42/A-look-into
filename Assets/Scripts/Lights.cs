using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lights:MonoBehaviour
{
    public Light2D lightObject;
    public bool lightsOn;


    public virtual void Light()
    {
        lightObject.enabled = true;
        lightsOn = true;
    }
    public virtual void Extinguish()
    {
        lightObject.enabled = false;
        lightsOn = false;
    }
}
