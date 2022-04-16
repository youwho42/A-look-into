using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLifeCycle : MonoBehaviour
{
  
    [Serializable]
    public struct PlantCycle
    {
        public Sprite mainSprite;
        public SpriteRenderer shadowSprite;
        public Sprite spriteMask;
        
        public List<GameObject> birdLandingSpots;
        
        public float obstacleCollisionHeights;
    }

    DayNightCycle dayNightCycle;
    public List<PlantCycle> plantCycles = new List<PlantCycle>();
    public int daysPerCycle;
    public SpriteRenderer spriteDisplay;
    public SpriteMask spriteMask;
    TreeShadows shadow;
    public GameObject homePoint;


    public DrawZasYDisplacement heightDisplacement;

    
    public int currentCycle;
    public string homeOccupiedBy = "";

    
    public int currentDay;
    public int currentTimeTick;
    public int gatherableCycle;
    GatherableItem gatherableItem;
    public bool isAnimalShelter;
    
    private void Start()
    {
        if(TryGetComponent(out GatherableItem item))
            gatherableItem = item;

        dayNightCycle = DayNightCycle.instance;

        shadow = GetComponent<TreeShadows>();
        SetCurrentCycle();
        GameEventManager.onTimeTickEvent.AddListener(UpdateCycle);
    }
    private void OnBecameVisible()
    {
        GameEventManager.onTimeTickEvent.AddListener(UpdateCycle);
    }

    private void OnBecameInvisible()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(UpdateCycle);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(UpdateCycle);
    }

    void SetCurrentTimeDay(int day, int time)
    {
        currentDay = dayNightCycle.currentDayRaw;
        currentTimeTick = dayNightCycle.currentTimeRaw;
    }
    public void SetHomeOccupation()
    {
        if (homeOccupiedBy != "" && homeOccupiedBy != null)
        {
            
            var go = Instantiate(AllItemsDatabaseManager.instance.allItemsDatabase.GetItem(homeOccupiedBy).ItemPrefab, transform.position, Quaternion.identity);

            if (go.TryGetComponent(out IAnimal thisAnimal))
            {
                thisAnimal.SetHome(transform);
            }
        }
    }

    

    void UpdateCycle(int tick)
    {
        
        if (currentTimeTick == dayNightCycle.currentTimeRaw && currentDay + daysPerCycle == dayNightCycle.currentDayRaw)
        {
            currentCycle++;
            if (currentCycle == plantCycles.Count)
                currentCycle = plantCycles.Count -1;

            SetCurrentCycle();

            
        }
        
    }
    
    
    

    public void SetCurrentCycle()
    {
        for (int i = 0; i < plantCycles.Count; i++)
        {
            if(i == currentCycle)
            {
                shadow.shadowSprite = plantCycles[i].shadowSprite;

                if (plantCycles[i].spriteMask == null)
                    spriteMask.sprite = plantCycles[i].mainSprite;
                else
                    spriteMask.sprite = plantCycles[i].spriteMask;

                spriteDisplay.sprite = plantCycles[i].mainSprite;
                plantCycles[i].shadowSprite.gameObject.SetActive(true);
                if(heightDisplacement != null)
                    heightDisplacement.positionZ = plantCycles[i].obstacleCollisionHeights;

                foreach (var spot in plantCycles[i].birdLandingSpots)
                {
                    spot.SetActive(true);
                }
            }
            else
            {
                plantCycles[i].shadowSprite.gameObject.SetActive(false);
                
                foreach (var spot in plantCycles[i].birdLandingSpots)
                {
                    spot.SetActive(false);
                }
            }
        }
        if (gatherableItem != null)
        {
            if (currentCycle < gatherableCycle)
                gatherableItem.hasBeenHarvested = true;
            else
                gatherableItem.hasBeenHarvested = false;
        }
            


        if (TryGetComponent(out InteractablePickUp pickUp))
        {
            pickUp.canInteract = currentCycle >= plantCycles.Count - 1;
            PlayerInformation.instance.playerStats.AddGameEnergy(2);
        }
        
        
    }
}
