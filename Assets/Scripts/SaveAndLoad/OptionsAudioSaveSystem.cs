using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class OptionsAudioSaveSystem : MonoBehaviour, ISaveable
    {
        public AudioSettingsUI audioSettings;

        public object CaptureState()
        {
            
            return new SaveData
            {
                master = audioSettings.masterSlider.value,
                music = audioSettings.musicSlider.value,
                effects = audioSettings.fxSlider.value,
                animals = audioSettings.animalsSlider.value
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            audioSettings.SetFromSave(saveData.master, saveData.music, saveData.effects, saveData.animals);
        }

        [Serializable]
        private struct SaveData
        {
            public float master;
            public float music;
            public float effects;
            public float animals;

        }
    }
}
