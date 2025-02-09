using System;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class FixableObjectSaveSystem : MonoBehaviour, ISaveable
    {
        public FixableObject fixableObject;

        public object CaptureState()
        {
            return new SaveData
            {
                hasBeenFixed = fixableObject.hasBeenFixed
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            fixableObject.SetObjectFromSave(saveData.hasBeenFixed);

        }

        [Serializable]
        private struct SaveData
        {
            public bool hasBeenFixed;

        }
    } 
}
