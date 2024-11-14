using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLight : Lights
{

    SpriteRenderer fuzzGlowImage;
    public float bloomMinIntensity;
    public float bloomMaxIntensity;
    Material material;
    Color initialColor;

    private void Start()
    {
        fuzzGlowImage = GetComponentInChildren<SpriteRenderer>();
        material = fuzzGlowImage.material;
        initialColor = material.GetColor("_EmissionColor");
        Extinguish();
    }

    public override void Light()
    {
        StartCoroutine("SetLightsCo");
    }

    public override void Extinguish()
    {
        base.Extinguish();
        if (material != null)
            material.SetColor("_EmissionColor", initialColor * bloomMinIntensity);
    }

    IEnumerator SetLightsCo()
    {
        float r = Random.Range(0.0f, 1.0f);
        yield return new WaitForSeconds(r);
        lightObject.enabled = true;
        lightsOn = true;
        if (material != null)
            material.SetColor("_EmissionColor", initialColor * bloomMaxIntensity);
    }
}
