using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class GatherableItemSaveSystem : MonoBehaviour, ISaveable
    {

        GatherableItem gatherableItem;
        void Start()
        {
            gatherableItem = GetComponent<GatherableItem>();
        }
        public object CaptureState()
        {
            return new SaveData
            {
                hasBeenHarvested = gatherableItem.hasBeenHarvested
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            gatherableItem.SetStateFromSave(saveData.hasBeenHarvested);


        }

        [Serializable]
        private struct SaveData
        {
            public bool hasBeenHarvested;
        }
    }

}