using Klaxon.Interactable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class InteractableFissureSaveSystem : MonoBehaviour, ISaveable
	{
        public InteractableFissure fissure;

        public object CaptureState()
        {
            return new SaveData
            {
                isActive = fissure.isActivated
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            fissure.isActivated = saveData.isActive;
            fissure.SetGameObjectRainStates(fissure.isActivated);

        }

        [Serializable]
        private struct SaveData
        {
            public bool isActive;
        }
    } 
}
