using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSaveSystem : MonoBehaviour, ISaveable
{
    BasicAI basicAI;
    void Start()
    {
        basicAI = GetComponent<BasicAI>();
    }

    public object CaptureState()
    {

        return new SaveData
        {
            homeBaseTilePositionX = basicAI.homeBaseTilePosition.x,
            homeBaseTilePositionY = basicAI.homeBaseTilePosition.y,
            homeBaseTilePositionZ = basicAI.homeBaseTilePosition.z,
            currentState = (int)basicAI.currentState,
            isActivated = basicAI.isActivated
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
        basicAI.homeBaseTilePosition = new Vector3Int(saveData.homeBaseTilePositionX, saveData.homeBaseTilePositionY, saveData.homeBaseTilePositionZ);
        basicAI.currentState = (BasicAI.RobotStates)saveData.currentState;
        basicAI.isActivated = saveData.isActivated;
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
