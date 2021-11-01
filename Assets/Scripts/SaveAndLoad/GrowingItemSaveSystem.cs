using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingItemSaveSystem : MonoBehaviour, ISaveable
{
    public GrowingItem growingItem;



    public object CaptureState()
    {
        return new SaveData
        {
            currentTimeTick = growingItem.currentTimeTick
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        growingItem.currentTimeTick = saveData.currentTimeTick;
    }

    [Serializable]
    private struct SaveData
    {
        public int currentTimeTick;
    }
}
