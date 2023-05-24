using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class PlantingArea : MonoBehaviour
{

    PolygonCollider2D coll;
    public SeedItemData seedItem;
   
    public List<Vector3> plantFreeLocations = new List<Vector3>();
    public List<Vector3> plantUsedLocations = new List<Vector3>();
    public List<PlantLife> harvestablePlants = new List<PlantLife>();
    public QI_Inventory seedBox;
    public Vector2 seedBoxPosition;
    public bool canPlant;
    public bool canHarvest;
    
    public bool ballPersonPlanterActive;
    public bool ballPersonHarvesterActive;

    public bool farmAreaActive;
    public Tilemap farmAreaTilemap;
    public TileBase unactiveFarmTile;
    public TileBase activeFarmTile;
    public List<Vector3Int> farmTilePositions = new List<Vector3Int>();
    InteractableFixingArea interactable;
    //public Image signPost;
    private void Start()
    {
        interactable = GetComponent<InteractableFixingArea>();
        coll = GetComponent<PolygonCollider2D>();
        SetFarmAreaActive(farmAreaActive);
    }

    private void OnEnable()
    {
        GameEventManager.onInventoryUpdateEvent.AddListener(CheckForPlantable);
        GameEventManager.onTimeHourEvent.AddListener(SetWorkTime);
    }

    private void OnDisable()
    {
        GameEventManager.onInventoryUpdateEvent.RemoveListener(CheckForPlantable);
        GameEventManager.onTimeHourEvent.RemoveListener(SetWorkTime);
    }
    void SetWorkTime(int time)
    {
        if ((RealTimeDayNightCycle.instance.dayStart / 60) + 1 == time)
        { 
            CheckForPlantable();
            CheckForHarvestable();
        }
    }
    bool CheckTime()
    {
        if(RealTimeDayNightCycle.instance.currentTimeRaw >= RealTimeDayNightCycle.instance.dayStart + 60 && RealTimeDayNightCycle.instance.currentTimeRaw <= RealTimeDayNightCycle.instance.nightStart)
        {
            return true;
        }
        return false;
    }

    public void CheckForPlantable()
    {
        // check for free spot
        // check for available seeds > is it the right type for the plot

        canPlant = false;
        if (!CheckTime())
            return;
        if (seedBox.Stacks.Count == 0)
            return;
        foreach (var stack in seedBox.Stacks)
        {
            if (stack.Item.GetType() == typeof(SeedItemData))
            {
                if (seedItem == null)
                {
                    seedItem = stack.Item as SeedItemData;
                    SetPositions();
                    canPlant = true;
                    break;
                }
                else if (seedItem == stack.Item)
                {
                    canPlant = true;
                    break;
                }
                else if(seedItem != null && seedItem != stack.Item)
                {
                    if(plantUsedLocations.Count == 0)
                    {
                        ResetPlantingArea();
                        seedItem = stack.Item as SeedItemData;
                        SetPositions();
                        canPlant = true;
                        break;
                    }
                    
                }
            }
        }

        

        if (plantFreeLocations.Count == 0)
            canPlant = false;

        if (canPlant && !ballPersonPlanterActive)
        {
            ballPersonPlanterActive = true;
            Invoke("SpawnPlanter", 1f);
        }
        CheckForHarvestable();
    }

    public void CheckForHarvestable()
    {
        canHarvest = false;
        if (!CheckTime())
            return;
        if (harvestablePlants.Count == 0)
            return;
        
        if(harvestablePlants.Count <= plantUsedLocations.Count * 0.75f && !ballPersonHarvesterActive)
            return;
            
        canHarvest = true;
        int freeStacks = seedBox.MaxStacks - seedBox.Stacks.Count;
        int availableStacks = 0;
        foreach (var item in seedItem.plantedObject.harvestedItems)
        {
            foreach (var stack in seedBox.Stacks)
            {
                if(stack.Item == item.harvestedItem)
                {
                    if (stack.Item.MaxStack == 0)
                    {
                        availableStacks++;
                        break;
                    }
                    var currentAmount = seedItem.plantedObject.GetAmount(item.amountVariance, item.minMaxAmount);
                    int space = stack.Item.MaxStack - stack.Amount;
                    if (space >= currentAmount)
                    {
                        availableStacks++;
                        break;
                    }
                        
                }
            }
        }
        if (availableStacks + freeStacks < seedItem.plantedObject.harvestedItems.Length)
            canHarvest = false;
        if (canHarvest && !ballPersonHarvesterActive)
        {
            ballPersonHarvesterActive = true;
            Invoke("SpawnHarvester", 1f);
        }
        
    }

    void SpawnPlanter()
    {
        BallPeopleManager.instance.SpawnFarmPlanter(this, seedBox.transform.position);
    }
    void SpawnHarvester()
    {
        BallPeopleManager.instance.SpawnFarmHarvester(this, seedBox.transform.position);
    }

    void SetPositions()
    {
        
        Vector2 area = new Vector2(coll.bounds.size.x, coll.bounds.size.y);

        List<Vector2> positions = new List<Vector2>();
        positions = PoissonDiscSampler.GeneratePoints(seedItem.plantingDistance, area);
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 pos = positions[i] + (Vector2)coll.bounds.min;
            pos.z = transform.position.z;
            //Check if position is in the area
            if (coll.OverlapPoint(pos))
            {
                plantFreeLocations.Add(pos);
            }

        }

    }

    public void SetPlantingArea(SeedItemData seedItemData)
    {
        seedItem = seedItemData;
        SetPositions();

    }

    void ResetPlantingArea()
    {
        plantFreeLocations.Clear();
        plantUsedLocations.Clear();
        harvestablePlants.Clear();
        seedItem = null;
        
    }

    public void SetFarmAreaActive(bool isActive)
    {
        if(interactable != null)
            interactable.canInteract = !isActive;
        farmAreaActive = isActive;
        seedBox.gameObject.transform.localPosition = seedBoxPosition;
        seedBox.gameObject.SetActive(isActive);
        for (int i = 0; i < farmTilePositions.Count; i++)
        {
            farmAreaTilemap.SetTile(farmTilePositions[i], isActive ? activeFarmTile : unactiveFarmTile);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (farmTilePositions.Count == 0 || farmAreaTilemap == null)
            return;
        for (int i = 0; i < farmTilePositions.Count; i++)
        {
            var x = farmAreaTilemap.GetCellCenterWorld(farmTilePositions[i]);
            Gizmos.DrawWireSphere(x, 0.1f);
        }
    }
}
