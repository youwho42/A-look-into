using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSuperShapeActivator : MonoBehaviour
{
    [Range(0,18)]
    public int layerPosition;

    public CreateParticleSuperShape superShape;

    
    void SetSuperShapeParticle()
    {
        superShape.particleLayers[0].particles[layerPosition].active = true;
        superShape.TotalM++;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.position.z != transform.position.z)
            return;
        if (collision.CompareTag("Ball"))
            SetSuperShapeParticle();
    }


}
