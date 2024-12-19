using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;
namespace Klaxon.SaveSystem
{
    public class GravityItemLocationSaveSystem : MonoBehaviour, ISaveable
    {

        public CurrentTilePosition currentTilePosition;
        public GravityItemNew gravityItem;

        public object CaptureState()
        {
            return new SaveData
            {
                location = transform.position,
                level = gravityItem.currentLevel,
                tilePositionX = currentTilePosition.position.x,
                tilePositionY = currentTilePosition.position.y,
                tilePositionZ = currentTilePosition.position.z
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            
            transform.position = saveData.location;
            Vector3Int tilePos = new Vector3Int(saveData.tilePositionX, saveData.tilePositionY, saveData.tilePositionZ);
            currentTilePosition.position = tilePos;
            gravityItem.currentLevel = saveData.level;
        }

        [Serializable]
        private struct SaveData
        {
            public SVector3 location;
            public int level;
            public int tilePositionX;
            public int tilePositionY;
            public int tilePositionZ;
        }
    }

}