using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDDisplayUI : MonoBehaviour
{
    public Slider bounceSlider;
    public Slider gumptionSlider;
    public TextMeshProUGUI bounceText;
    public TextMeshProUGUI gumptionText;
    public TextMeshProUGUI agencyText;
    public TextMeshProUGUI sparksText;
    public RectTransform bounceUI, gumptionUI, agencyUI, sparkUI;
    Vector2 bouncePos, gumptionPos, agencyPos, sparksPos;
    float lastBounce;
    float lastGumption;
    int lastAgency;
    int lastSparks;
    float shakeAmount = 0.7f;
    float shakeTime = 0.1f;
    float decreaseFactor = 1.0f;
    float shakeDistance = 4f;


    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.PlayerHUD);
   
        GameEventManager.onStatUpdateEvent.AddListener(UpdateStatsUI);
        
        bouncePos = bounceUI.anchoredPosition;
        gumptionPos = gumptionUI.anchoredPosition;
        agencyPos = agencyUI.anchoredPosition;
        sparksPos = sparkUI.anchoredPosition;
        
        gameObject.SetActive(false);
        UpdateStatsUI();
    }
    
    private void OnDestroy()
    {
        GameEventManager.onStatUpdateEvent.RemoveListener(UpdateStatsUI);
    }

    private void UpdateStatsUI()
    {
        
        float newMaxBounce = PlayerInformation.instance.statHandler.GetStatMaxModifiedValue("Bounce");
        float newCurrentBounce = PlayerInformation.instance.statHandler.GetStatCurrentModifiedValue("Bounce");
        float newMaxGumption = PlayerInformation.instance.statHandler.GetStatMaxModifiedValue("Gumption");
        float newCurrentGumption = PlayerInformation.instance.statHandler.GetStatCurrentModifiedValue("Gumption");
        int newAgency = (int)PlayerInformation.instance.statHandler.GetStatMaxModifiedValue("Agency");
        int newSparks = PlayerInformation.instance.purse.GetPurseAmount();

        if(newCurrentBounce != lastBounce)
        {
            float diff = Mathf.Abs(newCurrentBounce - lastBounce);
            lastBounce = newCurrentBounce;
            if (gameObject.activeSelf)
                StartCoroutine(ShakeStatUI(bounceUI, bouncePos, diff));
        }
        if (newCurrentGumption != lastGumption)
        {
            float diff = Mathf.Abs(newCurrentGumption - lastGumption);
            lastGumption = newCurrentGumption;
            if (gameObject.activeSelf)
                StartCoroutine(ShakeStatUI(gumptionUI, gumptionPos, diff));
        }
        if (newAgency != lastAgency)
        {
            float diff = Mathf.Abs(newAgency - lastAgency);
            lastAgency = newAgency;
            if (gameObject.activeSelf)
                StartCoroutine(ShakeStatUI(agencyUI, agencyPos, diff));
        }
        if (newSparks != lastSparks)
        {
            
            float diff = Mathf.Abs(newSparks - lastSparks);
            lastSparks = newSparks;
            if (gameObject.activeSelf)
                StartCoroutine(ShakeStatUI(sparkUI, sparksPos, diff));
        }
        bounceSlider.maxValue = newMaxBounce;
        bounceSlider.value = lastBounce;
        gumptionSlider.maxValue = newMaxGumption;
        gumptionSlider.value = lastGumption;
        bounceText.text = $"<sprite anim=\"6,21,12\"> {Mathf.FloorToInt(newCurrentBounce)}/{Mathf.FloorToInt(newMaxBounce)}";
        gumptionText.text = $"<sprite name=\"Gumption\"> {Mathf.FloorToInt(newCurrentGumption)}/{Mathf.FloorToInt(newMaxGumption)}";
        agencyText.text = $"<sprite name=\"Agency\"> {lastAgency}";
        sparksText.text = $"<sprite anim=\"3,5,12\"> {lastSparks}";

    }

    public IEnumerator ShakeStatUI(RectTransform statObject, Vector2 originalPos, float diff)
    {
        float currentShakeAmount = shakeAmount;
        
        var shakeDuration = shakeTime * diff;
        while (shakeDuration > 0)
        {
            statObject.anchoredPosition = originalPos + new Vector2(Random.Range(-shakeDistance, shakeDistance), Random.Range(-shakeDistance, shakeDistance)) * currentShakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
            currentShakeAmount = Mathf.Lerp(currentShakeAmount, 0, Time.deltaTime * decreaseFactor);

            yield return null;
        }

        statObject.anchoredPosition = originalPos;
    }
    
}
