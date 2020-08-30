using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTimeAndDate : MonoBehaviour, ISaveable
{
    public object CaptureState()
    {
        return new SaveData
        {
            currentTimeRaw = DayNightCycle.instance.currentTimeRaw,
            currentDayRaw = DayNightCycle.instance.currentDayRaw
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        DayNightCycle.instance.currentTimeRaw = saveData.currentTimeRaw;
        DayNightCycle.instance.currentDayRaw = saveData.currentDayRaw;
    }

    [Serializable]
    private struct SaveData
    {
        public int currentTimeRaw;
        public int currentDayRaw;
    }
}
