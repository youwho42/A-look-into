using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class SeedSaveSystem : MonoBehaviour, ISaveable
    {
        GrowingItem growingItem;

        private void Start()
        {
            growingItem = GetComponent<GrowingItem>();
        }

        public object CaptureState()
        {
            return new SaveData
            {
                location = transform.position,
                currentDay = growingItem.currentDay,
                currentTimeTick = growingItem.currentTimeTick
            };
        }

        public void RestoreState(object state)
        {
            StartCoroutine(RestoreStateCo(state));

        }
        public IEnumerator RestoreStateCo(object state)
        {
            var saveData = (SaveData)state;
            yield return new WaitForSeconds(.6f);
            transform.position = saveData.location;
            growingItem.SetCurrentDayTime(saveData.currentDay, saveData.currentTimeTick);

        }

        [Serializable]
        private struct SaveData
        {
            public SVector3 location;
            public int currentDay;
            public int currentTimeTick;
        }
    } 
}
