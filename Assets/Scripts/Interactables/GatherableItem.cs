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

    private void Start()
    {
        currentAmount = maxPerDay;
        DayNightCycle.instance.FullHourEventCallBack.AddListener(DailyReset);
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

    public void DailyReset(int time)
    {
        if (time == timeToReset)
        {
            currentAmount = maxPerDay;
            hasBeenHarvested = false;
        }
        
    }
}
