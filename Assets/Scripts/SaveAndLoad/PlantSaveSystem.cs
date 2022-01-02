using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSaveSystem : MonoBehaviour, ISaveable
{
    PlantLifeCycle plantLifeCycle;

    public virtual void Start()
    {
        plantLifeCycle = GetComponent<PlantLifeCycle>();
    }

    public object CaptureState()
    {
        return new SaveData
        {
            location = transform.position,
            currentCycle = plantLifeCycle.currentCycle,
            currentTimeTick = plantLifeCycle.currentTimeTick,
            homeOccupiedBy = plantLifeCycle.homeOccupiedBy
            
        };
    }

    public void RestoreState(object state)
    {
        StartCoroutine(RestoreStateCo(state));
    }
    IEnumerator RestoreStateCo(object state)
    {
        var saveData = (SaveData)state;
        
        yield return new WaitForSeconds(1f);
        transform.position = saveData.location;
        plantLifeCycle.currentCycle = saveData.currentCycle;
        plantLifeCycle.currentTimeTick = saveData.currentTimeTick;
        plantLifeCycle.homeOccupiedBy = saveData.homeOccupiedBy;
        plantLifeCycle.SetCurrentCycle();
        plantLifeCycle.SetHomeOccupation();
    }

    [Serializable]
    private struct SaveData
    {
        public SVector3 location;
        public int currentCycle;
        public int currentTimeTick;
        public string homeOccupiedBy;
    }
}
