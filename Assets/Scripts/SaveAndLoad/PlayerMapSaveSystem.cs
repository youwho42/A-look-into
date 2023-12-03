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
            foreach (var map in mapManager.mapAreas)
            {
                if (map.active)
                    maps.Add(map.mapName);
            }
            return new SaveData
            {
                mapNames = maps

            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;

            foreach (var map in saveData.mapNames)
            {
                mapManager.ActivateMapArea(map);
            }
        }

        [Serializable]
        private struct SaveData
        {
            public List<string> mapNames;
        }


    }

}