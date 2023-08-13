using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class RobotSaveSystem : MonoBehaviour, ISaveable
    {
        RobotAI robotAI;
        void Start()
        {
            robotAI = GetComponent<RobotAI>();
        }

        public object CaptureState()
        {

            return new SaveData
            {
                homeBaseTilePositionX = robotAI.homeBaseTilePosition.x,
                homeBaseTilePositionY = robotAI.homeBaseTilePosition.y,
                homeBaseTilePositionZ = robotAI.homeBaseTilePosition.z,
                currentState = (int)robotAI.currentState,
                isActivated = robotAI.isActivated
            };
        }

        public void RestoreState(object state)
        {
            StartCoroutine(RestoreStateCo(state));
        }
        public IEnumerator RestoreStateCo(object state)
        {
            var saveData = (SaveData)state;
            yield return new WaitForSeconds(0.35f);
            robotAI.homeBaseTilePosition = new Vector3Int(saveData.homeBaseTilePositionX, saveData.homeBaseTilePositionY, saveData.homeBaseTilePositionZ);
            robotAI.currentState = (RobotAI.RobotStates)saveData.currentState;
            robotAI.isActivated = saveData.isActivated;
        }

        [Serializable]
        private struct SaveData
        {
            public int homeBaseTilePositionX;
            public int homeBaseTilePositionY;
            public int homeBaseTilePositionZ;
            public int currentState;
            public bool isActivated;
        }
    }

}