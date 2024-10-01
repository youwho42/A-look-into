using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Localization;

public class Signpost : MonoBehaviour
{
    [Header("Text(s)")]
    public LocalizedString localizedTitle;
    public bool hasAlternateText;
    //[ConditionalHide("hasAlternateText", true)]
    public LocalizedString alternateLocalizedTitle;
    [ConditionalHide("hasAlternateText", true)]
    public UndertakingObject undertaking;
    [Header("Visuals")]
    public TextMeshProUGUI signpostText;
    public SpriteRenderer lightSprite;
    public SkewTextExample skewText;
    
    private void Start()
    {
        signpostText.text = localizedTitle.GetLocalizedString();
        if (hasAlternateText)
            GameEventManager.onUndertakingsUpdateEvent.AddListener(ChangeText);
    }
    private void OnDestroy()
    {
        if (hasAlternateText)
            GameEventManager.onUndertakingsUpdateEvent.RemoveListener(ChangeText);
    }

    void ChangeText()
    {
        if(undertaking != null)
        {
            if(undertaking.CurrentState == UndertakingState.Complete)
                signpostText.text = alternateLocalizedTitle.GetLocalizedString();
        }
        
    }

    private void OnBecameVisible()
    {
        StartCoroutine("FlickerSignLight");
        StartCoroutine("SkewSignText");
    }
    private void OnBecameInvisible()
    {
        StopCoroutine("FlickerSignLight");
        StopCoroutine("SkewSignText");
    }

    IEnumerator FlickerSignLight()
    {
        float timeBetweenFlickers = Random.Range(0.005f, 5f);
        while (true)
        {
            signpostText.enabled = true;
            lightSprite.enabled = true;
            yield return new WaitForSeconds(timeBetweenFlickers);
            timeBetweenFlickers = Random.Range(0.005f, 0.07f);
            signpostText.enabled = false;
            lightSprite.enabled = false;
            yield return new WaitForSeconds(timeBetweenFlickers);
            timeBetweenFlickers = Random.Range(0.005f, 5f);

            yield return null;

        }
    }
    IEnumerator SkewSignText()
    {
        float timeBetweenShears = Random.Range(0.005f, 4f);
        while (true)
        {
            skewText.CurveScale = 0;
            skewText.ShearAmount = 0;
            skewText.WarpText();
            yield return new WaitForSeconds(timeBetweenShears);
            timeBetweenShears = Random.Range(0.005f, 0.1f);
            skewText.CurveScale = Random.Range(-0.001f, 0.001f);
            skewText.ShearAmount = Random.Range(-50.0f, 50.0f);
            skewText.WarpText();
            yield return new WaitForSeconds(timeBetweenShears);
            timeBetweenShears = Random.Range(0.005f, 4f);

            yield return null;

        }
    }
}
