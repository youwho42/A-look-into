using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TreeShadows : MonoBehaviour
{
    public Transform shadowTransform;
    public SpriteRenderer shadowSprite;
    public List<SpriteRenderer> subShadowSprites = new List<SpriteRenderer>();
    List<Vector3> subShadowPositions = new List<Vector3>();
    List<Material> subShadowMaterials = new List<Material>();

    GlobalShadows globalShadows;
    bool isVisible;
    public ShadowCaster2D shadowCaster;
    Material shadowMaterial;
    [Range(1, 10)]
    public int shadowUpdateTick = 1;

    private void Awake()
    {
        if (shadowCaster != null)
            shadowCaster.enabled = false;
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        SetShadows(shadowUpdateTick);
    }
    private void OnBecameVisible()
    {
        for (int i = 0; i < subShadowSprites.Count; i++)
        {
            subShadowMaterials.Add(subShadowSprites[i].material);
            subShadowPositions.Add(subShadowSprites[i].transform.localPosition);
        }
        shadowMaterial = shadowSprite.material;
        globalShadows = GlobalShadows.instance;
        GameEventManager.onShadowTickEvent.AddListener(SetShadows);
        isVisible = true;
        SetShadows(shadowUpdateTick);
    }

    private void OnBecameInvisible()
    {
        
        GameEventManager.onShadowTickEvent.RemoveListener(SetShadows);
        isVisible = false;
        if (shadowCaster != null)
            shadowCaster.enabled = false;
         
    }
    private void OnDisable()
    {
        GameEventManager.onShadowTickEvent.RemoveListener(SetShadows);
    }

    public void SetShadows(int tick)
    {
        if (!isVisible)
            return;

        int sleep = 0;
        if (UIScreenManager.instance.isSleeping)
            sleep = 3;
        
        if (tick % (shadowUpdateTick + sleep) == 0)
            SetShadowState();
        
    }

    void SetShadowState()
    {
        bool visible = globalShadows.GetShadowVisible();
        Color c = globalShadows.GetShadowColor();
        var scale = globalShadows.shadowScale;
        for (int i = 0; i < subShadowSprites.Count; i++)
        {
            float diff = NumberFunctions.RemapNumber(scale.y, 0.4f, 1.4f, -0.08f, 0.08f);
            subShadowSprites[i].transform.localPosition = new Vector3(subShadowPositions[i].x, subShadowPositions[i].y + diff, subShadowPositions[i].z);
            subShadowSprites[i].enabled = visible;
            subShadowMaterials[i].SetColor("_Color", c);
            subShadowSprites[i].transform.localScale = scale;
        }
        shadowSprite.enabled = visible;
        shadowMaterial.SetColor("_Color", c);
        shadowTransform.eulerAngles = globalShadows.shadowRotation;
        shadowSprite.transform.localScale = scale;
        if (shadowCaster != null)
            shadowCaster.enabled = globalShadows.ShadowCasterEnabled();
            
    }
    
}
