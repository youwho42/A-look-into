using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;


public class PlantLife : MonoBehaviour
{
    [Serializable]
    public struct PlantCycle
    {
        public List<Sprite> plantSprites;
    }

    public SpriteRenderer mainSprite;
    public SpriteRenderer shadowSprite;

    public List<PlantCycle> plantCycles = new List<PlantCycle>();
    public int ticksPerCycle;


    public int currentCycle = 0;

    public CycleTicks cycle;
    //public int nextCycleDay;
    //public int nextCycleTick;

    InteractableFarmPlant interactablePlant;
    Collider2D coll;
    public LayerMask farmLayer;
    public float plantSwayAmount;
    private void OnEnable()
    {
        GameEventManager.onTimeTickEvent.AddListener(CheckPlantCycle);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(CheckPlantCycle);
    }

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        coll.enabled = false;
        interactablePlant = GetComponent<InteractableFarmPlant>();
        interactablePlant.canInteract = false;
        Material m = mainSprite.material;
        Material sm = null;
        if (shadowSprite != null)
            sm = shadowSprite.material;
        float r = UnityEngine.Random.Range(-.5f, .5f);
        m.SetFloat("_WindDensity", plantSwayAmount + r);
        if(sm!=null)
            sm.SetFloat("_WindDensity", plantSwayAmount + r);
        
    }

    void CheckPlantCycle(int tick)
    {
        if (currentCycle == plantCycles.Count - 1)
            return;
        RealTimeDayNightCycle dayNightCycle = RealTimeDayNightCycle.instance;
        if (tick >= cycle.tick && dayNightCycle.currentDayRaw == cycle.day)
        {
            currentCycle++;
            SetSprites();
            SetNextCycleTime();
        }
    }

    public void SetNextCycleTime()
    {
        cycle = RealTimeDayNightCycle.instance.GetCycleTime(ticksPerCycle);
    }

    public void SetSprites()
    {
        int rand = UnityEngine.Random.Range(0, plantCycles[currentCycle].plantSprites.Count);
        bool flip = UnityEngine.Random.Range(0.0f, 1.0f) > .5f ? true : false;
        mainSprite.sprite = plantCycles[currentCycle].plantSprites[rand];
        mainSprite.flipX = flip;
        if(shadowSprite != null)
        {
            shadowSprite.flipX = flip;
            if (currentCycle > 0)
                shadowSprite.sprite = plantCycles[currentCycle].plantSprites[rand];
        }
        
        if(currentCycle == plantCycles.Count - 1)
        {
            interactablePlant.canInteract = true;
            interactablePlant.plantingArea.harvestablePlants.Add(this);
            interactablePlant.plantingArea.CheckForHarvestable();
        }
            
        if(currentCycle > 0)
            coll.enabled = true;
    }

    public void SetPlantArea()
    {
        coll = GetComponent<Collider2D>();
        coll.enabled = false;
        interactablePlant = GetComponent<InteractableFarmPlant>();
        interactablePlant.canInteract = false;
        var hit = Physics2D.OverlapPoint(transform.position, farmLayer);
        
        if (hit != null)
        {
            interactablePlant.plantingArea = hit.GetComponent<PlantingArea>();
            interactablePlant.plantingArea.CheckForHarvestable();
        }

    }

    
}
