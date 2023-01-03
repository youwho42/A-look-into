using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTileLocationSaveSystem : MonoBehaviour, ISaveable
{

    public CurrentTilePosition currentTilePosition;

    public object CaptureState()
    {
        return new SaveData
        {
            location = transform.position,
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
