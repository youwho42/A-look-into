using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class OptionsGameplaySaveSystem : MonoBehaviour, ISaveable
    {
        public GameplayUI gameplayUI;

        public object CaptureState()
        {

            return new SaveData
            {
                localizationIndex = gameplayUI.localeDropdown.value,
                autoZoomBinary = gameplayUI.autoZoomBinary,
                HUDBinary = gameplayUI.HUDBinary,
                RIBBinary = gameplayUI.RIBBinary,
                AutoSaveBinary = gameplayUI.AutoSaveBinary
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            gameplayUI.SetFromSave(saveData.localizationIndex, saveData.autoZoomBinary, saveData.HUDBinary, saveData.RIBBinary, saveData.AutoSaveBinary);
        }

        [Serializable]
        private struct SaveData
        {
            public int localizationIndex;
            public int autoZoomBinary;
            public int HUDBinary;
            public int RIBBinary;
            public int AutoSaveBinary;
        }
    }
}
