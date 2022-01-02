using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductionSaveSystem : MonoBehaviour, ISaveable
{
    EntityReproduction reproduction;
    void Start()
    {
        reproduction = GetComponent<EntityReproduction>();
    }
    public object CaptureState()
    {
        return new SaveData
        {
            reproductionTick = reproduction.GetTick()
        };
    }

    public void RestoreState(object state)
    {
        StartCoroutine(RestoreStateCo(state));

    }
    
    public IEnumerator RestoreStateCo(object state)
    {
        var saveData = (SaveData)state;
        yield return new WaitForSeconds(0.3f);
        reproduction.SetTick(saveData.reproductionTick);

    }

    [Serializable]
    private struct SaveData
    {
        public int reproductionTick;
    }
}
