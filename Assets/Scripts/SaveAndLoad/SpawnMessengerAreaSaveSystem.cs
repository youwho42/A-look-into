using Klaxon.UndertakingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class SpawnMessengerAreaSaveSystem : MonoBehaviour, ISaveable
    {
        public SpawnBallPersonMessengerArea spawnArea;



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

}