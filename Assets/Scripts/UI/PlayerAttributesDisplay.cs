using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttributesDisplay : MonoBehaviour
{
    public Slider bounceSlider;
    public TextMeshProUGUI agencyText;
    public TextMeshProUGUI sparksText;
    public RectTransform bounceUI, agencyUI, sparkUI;
    Vector2 bouncePos, agencyPos, sparksPos;
    //PlayerInformation playerInformation;
    int lastBounce;
    int lastAgency;
    int lastSparks;
    float shakeAmount = 0.7f;
    float shakeTime = 0.1f;
    float decreaseFactor = 1.0f;
    float shakeDistance = 4f;
    private void Start()
    {
        //playerInformation = PlayerInformation.instance;
        GameEventManager.onStatUpdateEvent.AddListener(UpdateStatsUI);
        bouncePos = bounceUI.anchoredPosition;
        agencyPos = agencyUI.anchoredPosition;
        sparksPos = sparkUI.anchoredPosition;
    }
    //private void OnEnable()
    //{
    //    lastBounce = (int)PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Bounce");
    //    lastAgency = (int)PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Agency");
    //    bounceSlider.value = lastBounce;
    //    agencyText.text = lastAgency.ToString();
        
    //}
    private void OnDisable()
    {
        GameEventManager.onStatUpdateEvent.RemoveListener(UpdateStatsUI);
    }
    private void UpdateStatsUI()
    {
        if (!gameObject.activeSelf)
            return;
        int newBounce = (int)PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Bounce");
        int newAgency = (int)PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Agency");
        int newSparks = PlayerInformation.instance.purse.GetPurseAmount();

        if(newBounce != lastBounce)
        {
            float diff = Mathf.Abs(newBounce - lastBounce);
            lastBounce = newBounce;
            StartCoroutine(ShakeStatUI(bounceUI, bouncePos, diff));
        }
        if (newAgency != lastAgency)
        {
            float diff = Mathf.Abs(newAgency - lastAgency);
            lastAgency = newAgency;
            StartCoroutine(ShakeStatUI(agencyUI, agencyPos, diff));
        }
        if (newSparks != lastSparks)
        {
            float diff = Mathf.Abs(newSparks - lastSparks);
            lastSparks = newSparks;
            StartCoroutine(ShakeStatUI(sparkUI, sparksPos, diff));
        }
        bounceSlider.value = lastBounce;
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
