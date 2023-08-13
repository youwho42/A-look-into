using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class SpawnBallPersonSaveSystem : MonoBehaviour, ISaveable
    {
        public SpawnableBallPersonArea spawnArea;

        public object CaptureState()
        {
            return new SaveData
            {
                hasSpawned = spawnArea.GetHasSpawned()
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            spawnArea.SetHasSpawned(saveData.hasSpawned);
        }

        [Serializable]
        private struct SaveData
        {
            public bool hasSpawned;
        }
    } 
}