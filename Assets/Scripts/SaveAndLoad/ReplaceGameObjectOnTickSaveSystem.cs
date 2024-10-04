using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class ReplaceGameObjectOnTickSaveSystem : MonoBehaviour, ISaveable
	{
        public ReplaceGameObjectOnTick toggleScript;
        public object CaptureState()
        {
            return new SaveData
            {
                tick = toggleScript.cycle.tick,
                day = toggleScript.cycle.day
                
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            toggleScript.SetToggleFromSave(saveData.tick, saveData.day);

        }

        [Serializable]
        private struct SaveData
        {
            public int tick;
            public int day;
            
        }
    } 
}
