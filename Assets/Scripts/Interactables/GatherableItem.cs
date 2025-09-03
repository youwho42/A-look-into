using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherableItem : MonoBehaviour
{
    
    public List<QI_ItemData> dataList;
    [SerializeField]
    private int maxPerDay;
    private int currentAmount;
    public int timeToReset;
    public bool hasBeenHarvested;
    
    public GameObject harvestedSticker;
    public GameObject harvstedItemsHolder;

    private void Start()
    {
        currentAmount = maxPerDay;
        GameEventManager.onTimeTickEvent.AddListener(DailyReset);
    }

    private void OnDestroy()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(DailyReset);
    }
    public bool RemoveItem()
    {
        if (maxPerDay == 0)
            return true;
        if (currentAmount > 0)
        {
            currentAmount--;
            return true;
        }
        return false;
    }

    public void SetAsHarvested()
    {
        hasBeenHarvested = true;
        Invoke("SetHarvestVisuals", 2.0f);
    }

    void SetHarvestVisuals()
    {
        if (harvestedSticker != null)
            harvestedSticker.SetActive(hasBeenHarvested);
        if (harvstedItemsHolder != null)
            harvstedItemsHolder.SetActive(!hasBeenHarvested);
    }

    public void DailyReset(int time)
    {
        if (time == timeToReset)
        {
            currentAmount = maxPerDay;
            hasBeenHarvested = false;
            SetHarvestVisuals();
        }
        
    }

    public void SetStateFromSave(bool state)
    {
        hasBeenHarvested = state;
        SetHarvestVisuals();
    }
}
