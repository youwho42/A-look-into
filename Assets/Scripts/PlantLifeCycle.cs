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
        public int timeTickPerCycle;
        public List<GameObject> birdLandingSpots;
    }

    public List<PlantCycle> plantCycles = new List<PlantCycle>();
    public SpriteRenderer spriteDisplay;
    public SpriteMask spriteMask;
    TreeShadows shadow;
    public GameObject homePoint;
    
    public int currentCycle;
    public string homeOccupiedBy = "";

    public int currentTimeTick;
    public int gatherableCycle;
    GatherableItem gatherableItem;
    public bool isAnimalShelter;
    
    private void Start()
    {
        if(TryGetComponent(out GatherableItem item))
            gatherableItem = item;

        
        GameEventManager.onTimeTickEvent.AddListener(GetCurrentTime);
        shadow = GetComponent<TreeShadows>();
        SetCurrentCycle();
       
        
    }
    
    private void OnDestroy()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(GetCurrentTime);
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
        
        if (currentTimeTick >= plantCycles[currentCycle].timeTickPerCycle)
        {
            currentCycle++;
            if (currentCycle == plantCycles.Count)
                currentCycle = plantCycles.Count -1;

            SetCurrentCycle();

            currentTimeTick = 0;
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
