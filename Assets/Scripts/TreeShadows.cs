using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TreeShadows : MonoBehaviour
{
    public Transform shadowTransform;
    public SpriteRenderer shadowSprite;

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
        if (SleepDisplayUI.instance.isSleeping)
            sleep = 3;
        
        if (tick % (shadowUpdateTick + sleep) == 0)
            SetShadowState();
        
    }

    void SetShadowState()
    {
        shadowSprite.enabled = globalShadows.GetShadowVisible();
        shadowMaterial.SetColor("_Color", globalShadows.GetShadowColor());
        shadowTransform.eulerAngles = globalShadows.shadowRotation;
        shadowSprite.transform.localScale = globalShadows.shadowScale;
        if (shadowCaster != null)
            shadowCaster.enabled = globalShadows.ShadowCasterEnabled();
            
    }
    
}
