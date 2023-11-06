using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class PurseSaveSystem : MonoBehaviour, ISaveable
    {
        public PlayerPurse playerPurse;

        public object CaptureState()
        {

            return new SaveData
            {
                amount = playerPurse.GetPurseAmount()

            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;

            playerPurse.SetPurseAmount(saveData.amount);
            GameEventManager.onStatUpdateEvent.Invoke();
        }

        [Serializable]
        private struct SaveData
        {
            public int amount;
        }
    }

}