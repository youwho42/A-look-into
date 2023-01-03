using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TreeShadows : MonoBehaviour
{
    public Transform shadowTransform;
    public SpriteRenderer shadowSprite;

    bool isVisible = true;
    public ShadowCaster2D shadowCaster;
    private void Start()
    {
        
        GameEventManager.onTimeTickEvent.AddListener(SetShadows);
        SetShadows(0);
        if (shadowCaster != null)
        {
            shadowCaster.enabled = false;
        }
    }
    private void OnBecameVisible()
    {
        
        isVisible = true;
        SetShadows(0);
    }

    private void OnBecameInvisible()
    {
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
        shadowSprite.enabled = GlobalShadows.instance.GetShadowVisible();
        shadowSprite.color = GlobalShadows.instance.GetShadowColor();
        shadowTransform.eulerAngles = GlobalShadows.instance.shadowRotation;
        shadowSprite.transform.localScale = GlobalShadows.instance.shadowScale;
        if (shadowCaster != null)
        {
            shadowCaster.enabled = shadowSprite.color.a <= 0.2f;
        }
    }
    
}
