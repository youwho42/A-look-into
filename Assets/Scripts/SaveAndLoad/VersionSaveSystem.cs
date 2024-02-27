using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Klaxon.SaveSystem
{
    public class VersionSaveSystem : MonoBehaviour, ISaveable
    {

        public VersionDisplay versionDisplay;

        public object CaptureState()
        {
            
            int[] asIntegers = Application.version.Split('.').Select(s => int.Parse(s)).ToArray();
            
            return new SaveData { 
           
                version = Application.version,
                intVersion = asIntegers
            };
        }

        
        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            versionDisplay.savedVersion = saveData.version;
            versionDisplay.SetVersion(saveData.intVersion);
        }

        [Serializable]
        private struct SaveData
        {
            public string version;
            public int[] intVersion;
            
        }
    }
}
