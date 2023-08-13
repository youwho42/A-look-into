using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class PlantSaveSystem : MonoBehaviour, ISaveable
    {
        PlantGrowCycle plantGrowCycle;

        public virtual void Start()
        {
            plantGrowCycle = GetComponent<PlantGrowCycle>();
        }

        public object CaptureState()
        {
            return new SaveData
            {
                location = transform.position,
                currentCycle = plantGrowCycle.currentCycle,
                timeTickPlanted = plantGrowCycle.timeTickPlanted,
                dayPlanted = plantGrowCycle.dayPlanted,
                homeOccupiedBy = plantGrowCycle.homeOccupiedBy

            };
        }

        public void RestoreState(object state)
        {
            StartCoroutine(RestoreStateCo(state));
        }
        IEnumerator RestoreStateCo(object state)
        {
            var saveData = (SaveData)state;

            yield return new WaitForSeconds(1f);
            transform.position = saveData.location;
            plantGrowCycle.currentCycle = saveData.currentCycle;
            plantGrowCycle.timeTickPlanted = saveData.timeTickPlanted;
            plantGrowCycle.dayPlanted = saveData.dayPlanted;
            plantGrowCycle.homeOccupiedBy = saveData.homeOccupiedBy;
            plantGrowCycle.SetCurrentCycle();
            plantGrowCycle.SetHomeOccupation();
        }

        [Serializable]
        private struct SaveData
        {
            public SVector3 location;
            public int currentCycle;
            public int timeTickPlanted;
            public int dayPlanted;
            public string homeOccupiedBy;
        }
    }

}