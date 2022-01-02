using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedSaveSystem : MonoBehaviour, ISaveable
{
    GrowingItem growingItem;

    private void Start()
    {
        growingItem = GetComponent<GrowingItem>();
    }

    public object CaptureState()
    {
        return new SaveData
        {
            location = transform.position,
            currentTick = growingItem.GetCurrentTick()
        };
    }

    public void RestoreState(object state)
    {
        StartCoroutine(RestoreStateCo(state));

    }
    public IEnumerator RestoreStateCo(object state)
    {
        var saveData = (SaveData)state;
        yield return new WaitForSeconds(.6f);
        transform.position = saveData.location;
        growingItem.SetCurrentTick(saveData.currentTick);

    }

    [Serializable]
    private struct SaveData
    {
        public SVector3 location;
        public int currentTick;
    }
}
