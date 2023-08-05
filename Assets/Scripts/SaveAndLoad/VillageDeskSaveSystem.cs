using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageDeskSaveSystem : MonoBehaviour, ISaveable
{
    public FixVillageDesk villageDesk;

    public object CaptureState()
    {
        
        List<int> timer = new List<int>();
        List<bool> fixing = new List<bool>();
        List<bool> active = new List<bool>();
        foreach (var area in villageDesk.fixableAreas)
        {
            timer.Add(area.fixTimer);
            fixing.Add(area.isFixing);
            active.Add(area.isActive);
        }

        return new SaveData
        {
            fixTimer = timer,
            isFixing = fixing,
            isActive = active
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        for (int i = 0; i < villageDesk.fixableAreas.Count; i++)
        {
            villageDesk.fixableAreas[i].fixTimer = saveData.fixTimer[i];
            villageDesk.fixableAreas[i].isFixing = saveData.isFixing[i];
            villageDesk.fixableAreas[i].isActive = saveData.isActive[i];
        }
        villageDesk.SetAllAreas();
    }

    [Serializable]
    private struct SaveData
    {
        public List<int> fixTimer;
        public List<bool> isFixing;
        public List<bool> isActive;
    }
}
