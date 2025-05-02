using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    
    Material shadowMaterial;
    [Range(1, 10)]
    public int shadowUpdateTick = 1;
    bool nightShadowsEnabled;
    public Transform nightShadows;
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
            {
                OnBecameVisible();
                OnBecameInvisible();
            }
        }
        SetShadows(shadowUpdateTick);
        
    }
    private void OnBecameVisible()
    {
        shadowTransform.gameObject.SetActive(true);
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
    void CheckForLights()
    {
        if (nightShadows == null)
            return;
        if (nightShadowsEnabled && !globalShadows.ShadowCasterEnabled())
        {
            nightShadowsEnabled = false;
            nightShadows.gameObject.SetActive(false);
            return;
        }

        
        Light2D closestLightSource = globalShadows.GetClosestLightSource(nightShadows.transform.position);
        
        if(closestLightSource != null)
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
        SetNightShadowRotationAndZ(closestLightSource);
        if (!nightShadowsEnabled)
        {
            nightShadowsEnabled = true;
            nightShadows.gameObject.SetActive(true);
        }
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
