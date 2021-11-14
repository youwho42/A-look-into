using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCycleSaveSystem : MonoBehaviour, ISaveable
{
    public PlantLifeCycle lifeCycle;

    

    public object CaptureState()
    {
        
        return new SaveData
        {
            currentCycle = lifeCycle.currentCycle,
            currentTimeTick = lifeCycle.currentTimeTick,
            homeOccupiedBy = lifeCycle.homeOccupiedBy
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        lifeCycle.currentCycle = saveData.currentCycle;
        lifeCycle.currentTimeTick = saveData.currentTimeTick;
        lifeCycle.homeOccupiedBy = saveData.homeOccupiedBy;
        lifeCycle.SetCurrentCycle();
        lifeCycle.SetHomeOccupation();
    }
    


    [Serializable]
    private struct SaveData
    {
        public int currentCycle;
        public int currentTimeTick;
        public string homeOccupiedBy;
    }
}
