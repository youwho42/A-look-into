using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSaveSystem : MonoBehaviour, ISaveable
{

    public bool isPlayer;
    public object CaptureState()
    {
        return new SaveData
        {
            location = transform.position
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        transform.position = saveData.location;
        /*if (isPlayer)
        {
            //SetCollisionLayers.instance.SetCollisionLayer();
            FindObjectOfType<PlayerLevelChange>().UpdatePlayerLocation();
        }*/
            
    }

    [Serializable]
    private struct SaveData
    {
        public SVector3 location;
    }
}
