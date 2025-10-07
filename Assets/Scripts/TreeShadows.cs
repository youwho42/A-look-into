using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TreeShadows : MonoBehaviour
{
    public Transform shadowTransform;
    public SpriteRenderer shadowSprite;
    public SpriteRenderer nightShadowSprite;
    public List<SpriteRenderer> subShadowSprites = new List<SpriteRenderer>();
    //List<Vector3> subShadowPositions = new List<Vector3>();
    List<Material> subShadowMaterials = new List<Material>();

    public List<SpriteRenderer> subNightShadowSprites = new List<SpriteRenderer>();
    //List<Vector3> subNightShadowPositions = new List<Vector3>();

    

    GlobalShadows globalShadows;
    bool isVisible;
    
    Material shadowMaterial;
    [Range(1, 10)]
    public int shadowUpdateTick = 1;
    bool nightShadowsEnabled;
    public Transform nightShadows;

    bool materialsSet;
    //public bool isUnderCloud;

    private void Awake()
    {
        
        if(nightShadows != null)
            nightShadows.gameObject.SetActive(false);
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        if(TryGetComponent(out SpriteRenderer rend))
        {
            if (rend.isVisible)
                OnBecameVisible();
            else
                OnBecameInvisible();
        }
        SetShadows(shadowUpdateTick);
        
    }
    private void OnBecameVisible()
    {
        
        shadowTransform.gameObject.SetActive(true);
        if (!materialsSet)
        {
            foreach (var shadow in subNightShadowSprites)
            {
                shadow.material.SetColor("_Color", new Color(0, 0, 0, 0.3f));
            }
            nightShadowSprite.material.SetColor("_Color", new Color(0, 0, 0, 0.3f));

            for (int i = 0; i < subShadowSprites.Count; i++)
            {
                subShadowMaterials.Add(subShadowSprites[i].material);
            }

            materialsSet = true;
        }

        shadowMaterial = shadowSprite.material;


        globalShadows = GlobalShadows.instance;
        GameEventManager.onShadowTickEvent.AddListener(SetShadows);
        isVisible = true;
        SetShadows(shadowUpdateTick);
        GameEventManager.onLightsToggleEvent.AddListener(CheckForLights);
        CheckForLights();
    }

    
    private void OnBecameInvisible()
    {
        shadowTransform.gameObject.SetActive(false);
        GameEventManager.onShadowTickEvent.RemoveListener(SetShadows);
        isVisible = false;
        
        GameEventManager.onLightsToggleEvent.RemoveListener(CheckForLights);

    }
    private void OnDisable()
    {
        GameEventManager.onShadowTickEvent.RemoveListener(SetShadows);
        GameEventManager.onLightsToggleEvent.RemoveListener(CheckForLights);
        StopAllCoroutines();
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
            subShadowSprites[i].enabled = visible;
            subShadowMaterials[i].SetColor("_Color", c);
        }
        shadowSprite.enabled = visible;
        shadowMaterial.SetColor("_Color", c);
        shadowTransform.eulerAngles = globalShadows.shadowRotation;
        
        shadowTransform.localScale = scale;

        
    }
    public void CheckForLights()
    {
        if (nightShadows == null)
            return;
        if (nightShadowsEnabled && !globalShadows.ShadowCasterEnabled())
        {
            nightShadowsEnabled = false;
            nightShadows.gameObject.SetActive(false);
            return;
        }
        if (!globalShadows.ShadowCasterEnabled())
            return;

        Light2D closestLightSource = globalShadows.GetClosestLightSource(nightShadows.transform.position);
        
        if (closestLightSource != null)
        {
            SetNightShadows(closestLightSource);
        }
        else
        {
            nightShadowsEnabled = false;
            nightShadows.gameObject.SetActive(false);
        }
    }
    
    private void SetNightShadows(Light2D closestLightSource)
    {
        nightShadows.transform.localScale = new Vector3(1, 1.4f, 1);
        var flicker = closestLightSource.gameObject.GetComponentInParent<FireFlicker>();
        if (flicker != null)
            StartCoroutine(SetShadowFlickerCo(closestLightSource, flicker));

        SetNightShadowRotationAndZ(closestLightSource);
        if (!nightShadowsEnabled)
        {
            nightShadowsEnabled = true;
            nightShadows.gameObject.SetActive(true);
        }
        
    }
    IEnumerator SetShadowFlickerCo(Light2D closestLightSource, FireFlicker flicker)
    {
        while (globalShadows.ShadowCasterEnabled())
        {
            float f = closestLightSource.intensity;
            var s = NumberFunctions.RemapNumber(f, flicker.startIntensity - flicker.flickerAmount, flicker.startIntensity + flicker.flickerAmount, 0.03f, -0.03f);
            nightShadows.transform.localScale = new Vector3(1, 1.4f + s, 1);
            yield return null;
        }
        yield return null;
    }
    private void SetNightShadowRotationAndZ(Light2D closestLightSource)
    {
        Vector3 direction = nightShadows.position - closestLightSource.transform.position;
        nightShadows.rotation = Quaternion.LookRotation(Vector3.forward, direction);



        var z = Mathf.Abs(nightShadows.eulerAngles.z - 180);

        if (z <= 100)
        {
            z = NumberFunctions.RemapNumber(z, 0.0f, 180.0f, 2.1f, 0.3f);
            z = Mathf.Clamp(z, 0.0f, 2.1f);
        }
        else
            z = 0;

        nightShadows.localPosition = new Vector3(nightShadows.localPosition.x, nightShadows.localPosition.y, z);
    }

    

    
}
