using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Wind"))
        {
            Debug.Log("Wind touching stuff");
        } 
    }
    private void OnParticleTrigger()
    {
        
    }

}
