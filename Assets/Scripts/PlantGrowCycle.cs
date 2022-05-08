using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowCycle : MonoBehaviour
{



    public Sprite seedIdleSprite;
    public Sprite seedGrowSprite;
    
    
    [Serializable]
    public struct PlantCycle
    {
        
        public Sprite mainSprite;
        public SpriteRenderer shadowSprite;
        public Sprite spriteMask;

        public List<GameObject> birdLandingSpots;

        public float obstacleCollisionHeights;
    }

    bool isVisible = true;
    bool isSeed;
    public float seedCheckRadius;
    
    public List<PlantCycle> plantCycles = new List<PlantCycle>();
    public int daysPerCycle;

    public SpriteRenderer spriteDisplay;
    public SpriteMask spriteMask;
    TreeShadows shadow;
    public GameObject shadowHolder;
    public GameObject homePoint;


    public DrawZasYDisplacement heightDisplacement;


    RealTimeDayNightCycle dayNightCycle;
    public int currentCycle;
    public string homeOccupiedBy = "";

    [HideInInspector]
    public int dayPlanted;
    [HideInInspector]
    public int timeTickPlanted;
    public int gatherableCycle;
    GatherableItem gatherableItem;
    public bool isAnimalShelter;

    void Initialize()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;
        shadow = GetComponent<TreeShadows>();
    }
    private void Start()
    {
        if (TryGetComponent(out GatherableItem item))
            gatherableItem = item;

        dayNightCycle = RealTimeDayNightCycle.instance;

        shadow = GetComponent<TreeShadows>();
        //SetCurrentCycle();
        GameEventManager.onTimeTickEvent.AddListener(UpdateCycle);
    }
    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(UpdateCycle);
    }


    public void UpdateCycle(int tick)
    {
        if (dayNightCycle == null)
            Initialize();
        if (!isVisible)
            return;
        // if the plant is fully grown get out of here
        if (currentCycle >= plantCycles.Count)
            return;

        // get the amount of days that have passed since last update
        int daysPassed = 1 - dayPlanted;

        
        // more days have passed than the total plant cylces, set the plant to its final cycle and get out of here
        if (daysPassed >= plantCycles.Count)
        {
            currentCycle = plantCycles.Count;
            SetCurrentCycle();
            return;
        }

        // one or more days have passed than the days per cycle
        if (daysPassed >= daysPerCycle)
        {
            // it's another day, but it is not past the time of day at which it was planted in the first place
            if (dayNightCycle.currentTimeRaw <= timeTickPlanted)
            {
                daysPassed -= 1;
            }
            currentCycle = daysPassed;
            SetCurrentCycle();
        }
    }





    public void SetCurrentCycle()
    {
       
        // its still a seed, should it be 'planted' or 'idle'
        if (currentCycle == 0)
        {
            spriteDisplay.sprite = CanSeedGrow() ? seedGrowSprite : seedIdleSprite;
            shadowHolder.SetActive(false);
            spriteMask.sprite = null;
            if(heightDisplacement != null)
                heightDisplacement.gameObject.SetActive(false);
            foreach (var cycle in plantCycles)
            {
                foreach (var spot in cycle.birdLandingSpots)
                {
                    spot.SetActive(false);
                }
            }
            return;
        }

        // its not a seed anymore, set the plant cycle, shadows and all that jazz
        for (int i = 0; i < plantCycles.Count; i++)
        {
            if (i == currentCycle - 1)
            {
                if (heightDisplacement != null)
                    heightDisplacement.gameObject.SetActive(true);

                shadowHolder.SetActive(true);
                shadow.shadowSprite = plantCycles[i].shadowSprite;

                if (plantCycles[i].spriteMask == null)
                    spriteMask.sprite = plantCycles[i].mainSprite;
                else
                    spriteMask.sprite = plantCycles[i].spriteMask;

                spriteDisplay.sprite = plantCycles[i].mainSprite;
                plantCycles[i].shadowSprite.gameObject.SetActive(true);
                if (heightDisplacement != null)
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
            
        }


    }







    public void SetHomeOccupation()
    {
        if (homeOccupiedBy != "" && homeOccupiedBy != null)
        {

            var go = Instantiate(AllItemsDatabaseManager.instance.allItemsDatabase.GetItem(homeOccupiedBy).ItemPrefab, transform.position, Quaternion.identity);

            
        }
    }


    public bool CanSeedGrow()
    {
        
        var hit = Physics2D.OverlapCircleAll(transform.position, seedCheckRadius);
        if (hit.Length > 0)
        {
            foreach (var item in hit)
            {
                if (item.TryGetComponent(out PlantGrowCycle plant))
                {
                    dayPlanted = 0;
                    timeTickPlanted = dayNightCycle.currentTimeRaw;
                    return false;
                }

            }
        }
        
        return true;
    }





}
