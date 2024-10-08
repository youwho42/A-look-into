using System;
using System.Collections;
using Klaxon.Interactable;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class TempleFiresSaveSystem : MonoBehaviour, ISaveable
    {
        public InteractableTempleStation templeStation;

        public object CaptureState()
        {
            return new SaveData
            {
                isActive = templeStation.isActivated
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            templeStation.isActivated = saveData.isActive;
            templeStation.SetTempleFireAndRainStates(templeStation.isActivated);

        }

        [Serializable]
        private struct SaveData
        {
            public bool isActive;
        }
    }

}