using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class LocationSaveSystem : MonoBehaviour, ISaveable
    {

        public object CaptureState()
        {
            return new SaveData
            {
                location = transform.position
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            transform.position = saveData.location;

        }

        [Serializable]
        private struct SaveData
        {
            public SVector3 location;

        }
    }

}