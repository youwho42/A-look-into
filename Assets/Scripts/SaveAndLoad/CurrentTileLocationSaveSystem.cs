using Klaxon.GOAD;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class CurrentTileLocationSaveSystem : MonoBehaviour, ISaveable
    {

        public CurrentTilePosition currentTilePosition;
        public GOAD_Scheduler_NPC scheduler_NPC;

        public object CaptureState()
        {
            Vector3 position = Vector3.zero;
            if(scheduler_NPC != null)
            {
                if (scheduler_NPC.lastValidNode != null)
                    position = scheduler_NPC.lastValidNode.transform.position;
            }
                
            
            return new SaveData
            {
                location = position == Vector3.zero ? transform.position : position,
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
        }

        [Serializable]
        private struct SaveData
        {
            public SVector3 location;
            public int tilePositionX;
            public int tilePositionY;
            public int tilePositionZ;
        }
    }

}