using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class VersionSaveSystem : MonoBehaviour, ISaveable
    {

        public VersionDisplay versionDisplay;

        public object CaptureState()
        {

            return new SaveData
            {
                version = Application.version
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            versionDisplay.savedVersion = saveData.version;
        }

        [Serializable]
        private struct SaveData
        {
            public string version;

        }
    }
}
