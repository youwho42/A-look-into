using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TreeShadows : MonoBehaviour
{
    public Transform shadowTransform;
    public SpriteRenderer shadowSprite;

    GlobalShadows globalShadows;
    bool isVisible = true;
    public ShadowCaster2D shadowCaster;


    private IEnumerator Start()
    {
        
        
        if (shadowCaster != null)
        {
            shadowCaster.enabled = false;
        }

        yield return new WaitForSeconds(1.0f);
        globalShadows = GlobalShadows.instance;
        GameEventManager.onTimeTickEvent.AddListener(SetShadows);
        SetShadows(0);
    }
    private void OnBecameVisible()
    {
        globalShadows = GlobalShadows.instance;
        GameEventManager.onTimeTickEvent.AddListener(SetShadows);
        isVisible = true;
        SetShadows(0);
    }

    private void OnBecameInvisible()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetShadows);
        isVisible = false;
        if (shadowCaster != null)
        {
            shadowCaster.enabled = false;
        }
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetShadows);
    }

    public void SetShadows(int time)
    {
        if (!isVisible)
            return;
        
        SetShadowState();
    }

    void SetShadowState()
    {
        shadowSprite.enabled = globalShadows.GetShadowVisible();
        shadowSprite.color = globalShadows.GetShadowColor();
        shadowTransform.eulerAngles = globalShadows.shadowRotation;
        shadowSprite.transform.localScale = globalShadows.shadowScale;
        if (shadowCaster != null)
        {
            shadowCaster.enabled = shadowSprite.color.a <= 0.2f;
        }
    }
    
}
