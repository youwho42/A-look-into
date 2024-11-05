using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class PlayerMapSaveSystem : MonoBehaviour, ISaveable
    {
        public PlayerMapsManager mapManager;
        public object CaptureState()
        {
            List<string> maps = new List<string>();
            List<bool> animated = new List<bool>();
            foreach (var map in mapManager.mapAreas)
            {
                if (map.active)
                {
                    maps.Add(map.mapName);
                    animated.Add(map.hasAnimated);
                }
                    
            }
            return new SaveData
            {
                mapNames = maps,
                hasAnimated = animated

            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;

            for (int i = 0; i < saveData.mapNames.Count; i++)
            {
                mapManager.ActivateMapAreaFromSave(saveData.mapNames[i], saveData.hasAnimated[i]);
            }
            
        }

        [Serializable]
        private struct SaveData
        {
            public List<string> mapNames;
            public List<bool> hasAnimated;
            
        }


    }

}