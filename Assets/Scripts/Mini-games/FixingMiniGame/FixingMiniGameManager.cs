using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Klaxon.Interactable;

public class FixingMiniGameManager : MonoBehaviour, IMinigame
{
    

    /*[Serializable]
    public struct IngredientToFix
    {
        public QI_ItemData item;
        public int amount;
    }*/

    [Serializable]
    public struct Gauge
    {
        public SpriteRenderer gaugeSprite;
        public SimpleRotate gaugeRotate;
        public Vector2Int angleRange;
        
    }

    [Serializable]
    public struct TargetArea
    {
        public SpriteRenderer areaSprite;
        public SimpleRotate areaRotate;
        public Vector2Int angleRange;
        public float angleDifference;
        public float angleBonusDifference;
        public MiniGameDificulty gameDificulty;
        public float gaugeRotationTime;
    }
    
   

    public MiniGameType miniGameType;

    public MiniGameDificulty currentDificulty;

    /*public List<IngredientToFix> ingredients = new List<IngredientToFix>();*/
    
    public List<TargetArea> targetAreas = new List<TargetArea>();
    TargetArea currentTarget;

    public Gauge gauge;

    public SpriteRenderer centerImage;
    public TextMeshProUGUI attemptsText;

    bool inTransition;
    /*bool canStartGame;*/
    GameObject objectToFix;

    int attemptsTotal;
    int attemptsCurrent;
  
    AudioSource source;
    [SerializeField]
    public SoundSet[] soundSets;



    Material material;
    Color initialIntensity;
    private void Start()
    {

        //source = GetComponent<AudioSource>();
        initialIntensity = gauge.gaugeSprite.material.color;
        
        ResetMiniGame();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !inTransition)
        {
            inTransition = true;
            StopGaugeRotation();
            float diff = CheckGaugePosition();
            if (diff <= currentTarget.angleDifference)
            {
                if (diff <= currentTarget.angleBonusDifference)
                {
                    attemptsCurrent += 2;
                   
                }
                else
                {
                    attemptsCurrent++;
                    
                }
                int tot = attemptsTotal - attemptsCurrent;
                attemptsText.text = tot.ToString();
                if (attemptsCurrent >= attemptsTotal)
                {
                    //StartCoroutine(FailFixingCo());
                }
                else
                {
                    
                    StartCoroutine(NextAttemptCo(true));
                    
                }
                
            }
            else
            {
                StartCoroutine(NextAttemptCo(false));
            }
                
            
        }
        
    }

    public void SetupMiniGame(QI_ItemData item, GameObject go, MiniGameDificulty gameDificulty)
    {
        InteractableFixingArea area = go.GetComponent<InteractableFixingArea>();
        if (!area.CheckForIngredients())
        {
            StartCoroutine(FailFixingCo());
        }
        else
        {
            centerImage.sprite = area.ingredients[0].item.Icon;
            attemptsCurrent = 0;
            attemptsTotal = area.ingredients[0].amount;
            attemptsText.text = attemptsTotal.ToString();
            objectToFix = go;
            currentDificulty = gameDificulty;
            SetDificultySections();
            SetPositions();
            StartGaugeRotation();
        }
    }

    void SetDificultySections()
    {
        
        foreach (var area in targetAreas)
        {
            if (area.gameDificulty == currentDificulty)
            {
                area.areaSprite.enabled = true;
                area.areaRotate.enabled = true;
                material = area.areaSprite.material;
                currentTarget = area;
            }
            else
            {
                area.areaSprite.enabled = false;
                area.areaRotate.enabled = false;
            }
        }
    }
    void StartGaugeRotation()
    {
        gauge.gaugeRotate.StartToFromRotation(gauge.angleRange.x, gauge.angleRange.y, currentTarget.gaugeRotationTime);
    }
    void StopGaugeRotation()
    {
        gauge.gaugeRotate.StopToFromRotation();
    }
    void SetPositions()
    {
        currentTarget.areaRotate.SetEulerRotation(UnityEngine.Random.Range(currentTarget.angleRange.x, currentTarget.angleRange.y));
        gauge.gaugeRotate.SetEulerRotation(gauge.angleRange.x);
        
    }
    float CheckGaugePosition()
    {
        float diff = gauge.gaugeRotate.transform.eulerAngles.z - currentTarget.areaRotate.transform.eulerAngles.z;
        return Mathf.Abs(diff);
    }

    public void ResetMiniGame()
    {
        transform.parent.gameObject.SetActive(false);
    }

    IEnumerator FailFixingCo()
    {
        
        yield return new WaitForSeconds(0.2f);
        MiniGameManager.instance.EndMiniGame(miniGameType);
    }

    IEnumerator GlowOn(Material materialToSet, int amount)
    {
        float elapsedTime = 0;
        float waitTime = 0.5f;

        while (elapsedTime < waitTime)
        {
            Color i = Color.Lerp(initialIntensity, initialIntensity * amount, (elapsedTime / waitTime));

            materialToSet.SetColor("_EmissionColor", i);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        materialToSet.SetColor("_EmissionColor", initialIntensity * amount);
        yield return null;
    }
    void GlowOff(Material materialToSet)
    {
        materialToSet.SetColor("_EmissionColor", initialIntensity);
    }

    IEnumerator NextAttemptCo(bool success)
    {
        if (success)
        {
            Vector3 pos = centerImage.transform.position;
            float timer = 0;
            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                centerImage.transform.Translate(Vector2.up * (5 + (timer*10)) * Time.deltaTime);
                
                yield return null;
            }
            centerImage.transform.position = pos;
        }
        yield return new WaitForSeconds(1.0f);
        SetPositions();
        yield return new WaitForSeconds(1.0f);
        inTransition = false;
        StartGaugeRotation();
    }

}
