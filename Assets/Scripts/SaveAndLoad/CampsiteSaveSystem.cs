using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class CampsiteSaveSystem : MonoBehaviour, ISaveable
	{
        public CampsiteManager campsiteManager;

        public object CaptureState()
        {
            
                return new SaveData
                {
                    hasVisitor = campsiteManager.hasVisitor,
                    campIndex = campsiteManager.campIndex,
                    characterName = campsiteManager.characterName,
                    lastDay = campsiteManager.lastDay
                };
            

        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            campsiteManager.SetCampsiteFromSave(saveData.hasVisitor, saveData.campIndex, saveData.characterName, saveData.lastDay);
        }

        [Serializable]
        private struct SaveData
        {
            public bool hasVisitor;
            public int campIndex;
            public string characterName;
            public int lastDay;
        }
    } 
}
