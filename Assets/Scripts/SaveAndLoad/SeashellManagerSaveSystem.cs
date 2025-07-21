using UnityEngine;
using System.Collections.Generic;
using System;
using SerializableTypes;

namespace Klaxon.SaveSystem
{
    public class SeashellManagerSaveSystem : MonoBehaviour, ISaveable
    {
        public SeashellManager seashellManager;


        public object CaptureState()
        {

            List<string> name = new List<string>();
            List<SVector3> pos = new List<SVector3>();
            for (int i = 0; i < seashellManager.seashellItemList.Count; i++)
            {
                name.Add(seashellManager.seashellItemList[i].Name);
                pos.Add(seashellManager.seashellPositionList[i]);
            }
            return new SaveData
            {
                shellNames = name,
                shellPositions = pos
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            seashellManager.SpawnFromSave(saveData.shellNames, saveData.shellPositions);
        }

        [Serializable]
        private struct SaveData
        {
            public List<string> shellNames;
            public List<SVector3> shellPositions;
        }
    } 
}