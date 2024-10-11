using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SuperEllipse
{
    public List<SphereParticle> particles;
    public float a = 0.2f;
    public float b = 0.1f;
    public float totalItems = 10;
    public float zBase = 0.2f;
    public float zVariance = 0.2f;
    public bool oscillateZ;
   
    public float n = 1.8f;
    
    public bool oscillateN;

    [Range(0, 1)]
    public float offset = .5f;
}
public class CreateParticleSuperEllipse : MonoBehaviour
{
    
    
    float twoPI = Mathf.PI * 2;
    
    
    
    
    
    
    readonly float spriteDisplacementY = 0.2990625f;
    public SphereParticle particle;
    
    public List<SuperEllipse> particleLayers = new List<SuperEllipse>();
    float animationSpeed = 1f;
    private void Start()
    {
        
        foreach (var layer in particleLayers)
        {
            float itemAngleIncrement = twoPI / layer.totalItems;

            for (float angle = 0; angle < twoPI; angle += itemAngleIncrement)
            {
                
                float na = 2 / layer.n;
                float newAngle = angle + (itemAngleIncrement * layer.offset);
                float x = Mathf.Pow(Mathf.Abs(Mathf.Cos(newAngle)), na) * layer.a * Mathf.Sign(Mathf.Cos(newAngle));
                float y = Mathf.Pow(Mathf.Abs(Mathf.Sin(newAngle)), na) * layer.b * Mathf.Sign(Mathf.Sin(newAngle));

                Vector3 pos = new Vector3(x, y, 0);
                SphereParticle go = Instantiate(particle, transform);
                go.transform.localPosition = pos;
                
                Vector3 disp = new Vector3(0, spriteDisplacementY * layer.zBase, layer.zBase);
                go.itemObject.localPosition = disp;

                float scl = NumberFunctions.RemapNumber(y, -layer.b, layer.b, 1.0f, 0.8f);
                Vector3 size = new Vector3(scl, scl, scl);
                go.itemObject.localScale = size;
                go.itemShadow.localScale = size;
                layer.particles.Add(go);
                
            }
            
        }


    }

    private void Update()
    {
        foreach (var layer in particleLayers)
        {
            float itemAngleIncrement = twoPI / layer.totalItems;
            for (int i = 0; i < layer.particles.Count; i++)
            {
                float angle = i * itemAngleIncrement + Time.time * animationSpeed;
                angle += (itemAngleIncrement * layer.offset);

                float na = 2 / layer.n;
                if (layer.oscillateN)
                {
                    float n = layer.n + Mathf.Sin(Time.time);
                    na = 2 / n;
                }
                
                float x = Mathf.Pow(Mathf.Abs(Mathf.Cos(angle)), na) * layer.a * Mathf.Sign(Mathf.Cos(angle));
                float y = Mathf.Pow(Mathf.Abs(Mathf.Sin(angle)), na) * layer.b * Mathf.Sign(Mathf.Sin(angle));

                Vector3 pos = new Vector3(x, y, 0);
                layer.particles[i].transform.localPosition = pos;

                if (layer.oscillateZ)
                {
                    float newZ = layer.zBase + NumberFunctions.RemapNumber(Mathf.Sin(Time.time + angle), -1, 1, 0, layer.zVariance);
                    Vector3 disp = new Vector3(0, spriteDisplacementY * newZ, newZ);
                    layer.particles[i].itemObject.localPosition = disp;
                }
                

                float scl = NumberFunctions.RemapNumber(y, -layer.b, layer.b, 1.0f, 0.8f);
                Vector3 size = new Vector3(scl, scl, scl);
                layer.particles[i].itemObject.localScale = size;
                layer.particles[i].itemShadow.localScale = size;

            }
        }
        
        
    }

}
