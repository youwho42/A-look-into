using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class DailyObjectSaveSystem : MonoBehaviour, ISaveable
	{
        public SpawnDailyObjects dailyObjects;
        public object CaptureState()
        {
            List<string> objs = dailyObjects.GetSpawnedItems();
            
            return new SaveData
            {
                
                spawnObjects = objs
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            
            for (int i = 0; i < saveData.spawnObjects.Count; i++)
            {
                dailyObjects.SpawnItemFromSave(i, saveData.spawnObjects[i]);
                
            }
            

        }

        [Serializable]
        private struct SaveData
        {
            public List<string> spawnObjects;
        }
    }

}