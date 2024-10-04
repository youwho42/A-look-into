using Klaxon.Interactable;
using Klaxon.SaveSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceGameObjectOnTick : MonoBehaviour
{
    
    public GameObject replacementGameObject;

    public int ticksUntilReplace;
    [HideInInspector]
    public CycleTicks cycle;
    
    RealTimeDayNightCycle dayNightCycle;

    QI_ItemData item;

    

    private void Start()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;
        cycle = dayNightCycle.GetCycleTime(ticksUntilReplace);
        item = GetComponent<QI_Item>().Data;
        GameEventManager.onTimeTickEvent.AddListener(CheckToggle);
    }
    private void OnDestroy()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(CheckToggle);
    }

    void CheckToggle(int tick)
    {
        
        if(dayNightCycle.currentTimeRaw >= cycle.tick && dayNightCycle.currentDayRaw == cycle.day)
        {
            var go = Instantiate(replacementGameObject, transform.position, Quaternion.identity);
            
            if (go.TryGetComponent(out SaveableItemEntity itemID))
                itemID.GenerateId();

            var replace = go.GetComponent<Interactable>().replaceObjectOnDrop;
            var orig = GetComponent<Interactable>().replaceObjectOnDrop;
            if (replace != null && orig != null)
                replace.grassObjects = new List<GameObject>(orig.grassObjects);

            if (go.TryGetComponent(out QI_Item replacementItem))
            {
                if (replacementItem.Data.placementGumption != null)
                    PlayerInformation.instance.statHandler.AddModifiableModifier(replacementItem.Data.placementGumption);
            }

            PlayerInformation.instance.statHandler.RemoveModifiableModifier(item.placementGumption);
            Destroy(gameObject);

        }
    }

    public void SetToggleFromSave(int tick, int day)
    {
        cycle = new CycleTicks();
        cycle.tick = tick;
        cycle.day = day;
        
    }
}
