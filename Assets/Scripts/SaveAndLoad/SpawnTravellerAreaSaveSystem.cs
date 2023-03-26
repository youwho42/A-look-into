using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTravellerAreaSaveSystem : MonoBehaviour, ISaveable
{
    public SpawnBallPersonTravellerArea spawnArea;



    public object CaptureState()
    {
        return new SaveData
        {
            hasSpawned = spawnArea.hasSpawned
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        spawnArea.hasSpawned = saveData.hasSpawned;
    }

    [Serializable]
    private struct SaveData
    {
        public bool hasSpawned;
    }
}