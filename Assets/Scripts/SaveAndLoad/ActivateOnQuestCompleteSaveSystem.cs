using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class ActivateOnQuestCompleteSaveSystem : MonoBehaviour, ISaveable
    {
        public ActivateOnQuestComplete activateObject;
        public object CaptureState()
        {
            return new SaveData
            {
                undertakingName = activateObject.undertakingName
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            activateObject.undertakingName = saveData.undertakingName;

        }

        [Serializable]
        private struct SaveData
        {
            public string undertakingName;

        }
    } 
}
