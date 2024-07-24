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
                timesPoked = pokable.TimesPoked

            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;

            pokable.SetTimesPoked(saveData.timesPoked);
        }

        [Serializable]
        private struct SaveData
        {
            public int timesPoked;
        }
    }

} 
	