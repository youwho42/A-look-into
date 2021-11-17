using QuantumTek.EncryptedSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTimeCycleSystem : SaveableManager
{
    DayNightCycle timeCycle;

    private void Start()
    {
        timeCycle = DayNightCycle.instance;
    }

    public override void Save()
    {
        base.Save();
      
        ES_Save.Save(timeCycle.currentTimeRaw, saveableName + "time");
        ES_Save.Save(timeCycle.currentDayRaw, saveableName + "day");
        
    }
    public override void Load()
    {
        base.Load();
        
        StartCoroutine(LoadCo());
    }


    IEnumerator LoadCo()
    {

        yield return new WaitForSeconds(0.02f);
        timeCycle.SetDayTime(ES_Save.Load<int>(saveableName + "time"), ES_Save.Load<int>(saveableName + "day"));

    }

}
