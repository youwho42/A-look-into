using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostColorSetting : MonoBehaviour
{

    public SpriteRenderer ghostSprite;
    public SpriteRenderer shadowSprite;
    Material ghostMaterial;
    Material shadowMaterial;

    public Color ghostColor = Color.white;
    [ColorUsage(true, true)]
    public Color ghostBloom = Color.white;
    

    private void Start()
    {
        SetMaterials();
        SetColors();
    }

    private void SetMaterials()
    {
        ghostMaterial = ghostSprite.material;
        shadowMaterial = shadowSprite.material;
    }

    
    public void SetColors()
    {
        if(ghostMaterial == null || shadowMaterial == null)
            SetMaterials();
        ghostSprite.color = ghostColor;
        Color shadowColor = new Color(ghostColor.r, ghostColor.g, ghostColor.b, 0.8f);
        shadowSprite.color = shadowColor;
        ghostMaterial.SetColor("_EmissionColor", ghostBloom);
        shadowMaterial.SetColor("_EmissionColor", ghostBloom);
    }
}
