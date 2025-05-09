using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class OptionsGraphicsSaveSystem : MonoBehaviour, ISaveable
    {

        public GraphicsSettingsUI graphicsSettings;

        public object CaptureState()
        {
            return new SaveData
            {
                vSync = graphicsSettings.GetVSync(),
                fullscreen = graphicsSettings.GetFullscreen(),
                dropdownValue = graphicsSettings.GetLimitedFramerate()
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            graphicsSettings.SetFromSave(saveData.vSync, saveData.fullscreen, saveData.dropdownValue);
        }

        [Serializable]
        private struct SaveData
        {
            public int vSync;
            public bool fullscreen;
            public int dropdownValue;

        }
    }
}
