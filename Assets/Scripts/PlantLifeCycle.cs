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
        public int timeTickPerCycle;
        public List<GameObject> birdLandingSpots;
    }

    public List<PlantCycle> plantCycles = new List<PlantCycle>();
    public SpriteRenderer spriteDisplay;
    public SpriteMask spriteMask;
    TreeShadows shadow;
    public GameObject homePoint;
    DayNightCycle dayNightCycle;
    public int currentCycle;
    public string homeOccupiedBy = "";

    public int currentTimeTick;
    public int gatherableCycle;
    GatherableItem gatherableItem;
    
    private void Start()
    {
        if(TryGetComponent(out GatherableItem item))
            gatherableItem = item;

        dayNightCycle = DayNightCycle.instance;
        dayNightCycle.TickEventCallBack.AddListener(GetCurrentTime);
        shadow = GetComponent<TreeShadows>();
        SetCurrentCycle();
       
        
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

    void GetCurrentTime(int tick)
    {
       
        currentTimeTick++;
        if (currentCycle < plantCycles.Count)
            UpdateCycle();
    }

    void UpdateCycle()
    {
        
        if (currentTimeTick == plantCycles[currentCycle].timeTickPerCycle)
        {
            currentCycle++;
            if (currentCycle == plantCycles.Count)
                currentCycle = plantCycles.Count -1;

            SetCurrentCycle();

            currentTimeTick = 0;
        }
        if (currentCycle < plantCycles.Count - 1)
        {
            if(gatherableItem != null)
                gatherableItem.hasBeenHarvested = true;
        }
    }

    public void SetCurrentCycle()
    {
        for (int i = 0; i < plantCycles.Count; i++)
        {
            if(i == currentCycle)
            {
                shadow.shadowSprite = plantCycles[i].shadowSprite;
                spriteMask.sprite = plantCycles[i].mainSprite;
                spriteDisplay.sprite = plantCycles[i].mainSprite;
                plantCycles[i].shadowSprite.gameObject.SetActive(true);
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
        
        
    }
}
