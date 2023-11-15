using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SuperShape
{
    public List<SphereParticle> particles;
    public float n1, n2, n3 = 1;
    public float m = 1;
    public float a = 1;
    public float b = 1;



    public float totalItems = 10;
    public float zBase = 0.2f;
    public float zVariance = 0.2f;
    public bool oscillateZ;

    
    [Range(0, 1)]
    public float offset = .5f;

    public float animationSpeed = 1f;
    [Range(.001f, 20)]
    public float scale;
}



public class CreateParticleSuperShape : MonoBehaviour
{


    float twoPI = Mathf.PI * 2;






    readonly float spriteDisplacementY = 0.2990625f;
    public SphereParticle particle;

    public List<SuperShape> particleLayers = new List<SuperShape>();
    

    float SuperShape(SuperShape shape, float angle)
    {
        float p1 = Mathf.Abs((1 / shape.a) * Mathf.Cos(angle * shape.m / 4));
        p1 = Mathf.Pow(p1, shape.n2);

        float p2 = Mathf.Abs((1 / shape.b) * Mathf.Sin(angle * shape.m / 4));
        p2 = Mathf.Pow(p2, shape.n3);

        float p3 = Mathf.Pow(p1 + p2, 1 / shape.n1);

        return 1 / p3;
    }
    private void Start()
    {

        foreach (var layer in particleLayers)
        {
            float itemAngleIncrement = twoPI / layer.totalItems;

            for (float angle = 0; angle < twoPI - itemAngleIncrement; angle += itemAngleIncrement)
            {

                
                float newAngle = angle + (itemAngleIncrement * layer.offset);
                float r = SuperShape(layer, newAngle);

                float x = r * Mathf.Cos(newAngle);
                float y = r * Mathf.Sin(newAngle);

                Vector3 pos = new Vector3(x, y, 0);
                SphereParticle go = Instantiate(particle, transform);
                go.transform.localPosition = pos;

                Vector3 disp = new Vector3(0, spriteDisplacementY * layer.zBase, layer.zBase);
                go.itemObject.localPosition = disp;

                float scl = MapNumber.Remap(y, -layer.b, layer.b, 1.0f, 0.8f);
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
                float angle = i * itemAngleIncrement + Time.time * layer.animationSpeed;
                angle += (itemAngleIncrement * layer.offset);



                float r = SuperShape(layer, angle) * layer.scale;

                float x = r * Mathf.Cos(angle);
                float y = r * Mathf.Sin(angle);

                Vector3 pos = new Vector3(x, y, 0);
                layer.particles[i].transform.localPosition = pos;

                if (layer.oscillateZ)
                {
                    float newZ = layer.zBase + MapNumber.Remap(Mathf.Sin(Time.time + angle), -1, 1, 0, layer.zVariance);
                    Vector3 disp = new Vector3(0, spriteDisplacementY * newZ, newZ);
                    layer.particles[i].itemObject.localPosition = disp;
                }


                float scl = MapNumber.Remap(y, -layer.b, layer.b, 1.0f, 0.8f);
                Vector3 size = new Vector3(scl, scl, scl);
                layer.particles[i].itemObject.localScale = size;
                layer.particles[i].itemShadow.localScale = size;

            }
        }


    }

}