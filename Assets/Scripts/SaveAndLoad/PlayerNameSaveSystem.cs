using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerNameSaveSystem : MonoBehaviour, ISaveable
{
    public object CaptureState()
    {
        return new SaveData
        {
            playerName = PlayerInformation.instance.playerName
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        PlayerInformation.instance.SetPlayerName(saveData.playerName);

    }

    [Serializable]
    private struct SaveData
    {
        public string playerName;

    }
}
