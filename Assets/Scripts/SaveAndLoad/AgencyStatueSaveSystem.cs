using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgencyStatueSaveSystem : MonoBehaviour, ISaveable
{
    public InteractableAgencyStatue agencyStatue;


    public object CaptureState()
    {
        return new SaveData
        {
            hasInteracted = agencyStatue.hasBeenActivated
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        agencyStatue.hasBeenActivated = saveData.hasInteracted;
        if (agencyStatue.hasBeenActivated)
        {
            agencyStatue.canInteract = false;
            
        }
        agencyStatue.SetSaveColor(!agencyStatue.hasBeenActivated);
    }

    [Serializable]
    private struct SaveData
    {
        public bool hasInteracted;

    }
}