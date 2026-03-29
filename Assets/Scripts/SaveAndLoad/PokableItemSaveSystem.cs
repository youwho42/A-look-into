using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class PokableItemSaveSystem : MonoBehaviour, ISaveable
    {
        public PokableItem pokable;

        public object CaptureState()
        {

            return new SaveData
            {
                attempts = pokable.TotalAttemptedTimesPoked,
                success = pokable.SuccessfulTimesPoked,
                fails = pokable.FailedTimesPoked

            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;

            pokable.SetTimesPoked(saveData.attempts, saveData.success, saveData.fails);
        }

        [Serializable]
        private struct SaveData
        {
            public int attempts;
            public int success;
            public int fails;
        }
    }

} 
	