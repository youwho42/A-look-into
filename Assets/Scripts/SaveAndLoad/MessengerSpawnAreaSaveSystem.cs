using Klaxon.UndertakingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MessengerSpawnAreaSaveSystem : MonoBehaviour, ISaveable
{
    public SpawnMessengerArea spawnArea;



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
