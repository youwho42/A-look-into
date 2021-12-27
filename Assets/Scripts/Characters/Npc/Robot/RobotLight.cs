using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotLight : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Material material;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
        
    }

    public void SetIntensity(float intensity)
    {
        Color c = spriteRenderer.color * intensity;
        material.SetColor("_EmissionColor", c);
    }

}
