using UnityEngine;
using System.Collections.Generic;
using Klaxon.Interactable;
using Klaxon.GOAD;


public class AreaStartFires : MonoBehaviour
{

    public List<GameObject> allFires = new List<GameObject>();
    RealTimeDayNightCycle dayNightCycle;

    public GOAD_ScriptableCondition scriptableCondition;
    GOAD_WorldBeliefStates worldBeliefStates;

    private void Start()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;
        worldBeliefStates = GOAD_WorldBeliefStates.instance;
        GameEventManager.onDayStateChangeEvent.AddListener(StartFires);
    }
    private void OnDestroy()
    {
        GameEventManager.onDayStateChangeEvent.RemoveListener(StartFires);
    }

    void StartFires()
    {
        if (!worldBeliefStates.HasState(scriptableCondition.Condition, scriptableCondition.State))
            return;
        if(dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Sunset 
            || dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Sunrise)
        {
            
            bool light = dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Sunset;
            foreach (var fire in allFires)
            {
                Debug.Log($"start fires : {light}");
                fire.SetActive(light);
                
            }
        }
    }
}
