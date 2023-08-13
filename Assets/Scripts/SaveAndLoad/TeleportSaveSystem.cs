using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class TeleportSaveSystem : MonoBehaviour, ISaveable
    {
        Teleport teleporter;

        void Start()
        {
            teleporter = GetComponent<Teleport>();
        }
        public object CaptureState()
        {
            return new SaveData
            {
                currentLevel = teleporter.currentLevel,
                name = teleporter.teleportName
            };
        }

        public void RestoreState(object state)
        {
            StartCoroutine(RestoreStateCo(state));

        }
        public IEnumerator RestoreStateCo(object state)
        {
            var saveData = (SaveData)state;
            yield return new WaitForSeconds(0.5f);
            teleporter.currentLevel = saveData.currentLevel;
            teleporter.teleportName = saveData.name;
            teleporter.SetCurrentLevel();

        }

        [Serializable]
        private struct SaveData
        {
            public int currentLevel;
            public string name;
        }
    } 
}
