using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunningUI : MonoBehaviour
{
    public static RunningUI instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public RectTransform runUIObject;
    public Image colorSprite;
    public Image glass;
    public Image crack;
    public Image shattered;
    public Sprite[] cracks;
    public Color startColor;
    public Color endColor;

    float shakeAmount = 0.7f;
    float shakeTime = 0.1f;
    float decreaseFactor = 1.0f;
    float shakeDistance = 4f;
    Vector2 origPos;

    private void Start()
    {
        ToggleUI(false);
        SetCracks(-1);
        origPos = runUIObject.anchoredPosition;
    }

    public void ToggleUI(bool state)
    {
        runUIObject.gameObject.SetActive(state);
        if (state)
            ShatterGlass(false);
    }

    public void SetColor(float t)
    {
        Color c = Color.Lerp(startColor, endColor, t);
        colorSprite.fillAmount = t;
        colorSprite.color = c;
    }

    public void SetCracks(int index)
    {
        if(index == -1)
        {
            crack.gameObject.SetActive(false);
            crack.sprite = null;
            return;
        }
        ShakeStat(.2f * (index+2));
        
        
        crack.gameObject.SetActive(true);
        crack.sprite= cracks[index];
    }
    public void ShatterGlass(bool state)
    {
        glass.gameObject.SetActive(!state);
        shattered.gameObject.SetActive(state);
        if (state)
        {
            
            ShakeStat(2.5f);
        }
            
    }
    public void ShakeStat(float amount)
    {
        if(UIScreenManager.instance.gameplay.HUDBinary == 1)
            StartCoroutine(ShakeStatUI(runUIObject, origPos, amount));
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
