using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class PlayerCharacterSaveSystem : MonoBehaviour, ISaveable
    {
        public object CaptureState()
        {

            return new SaveData
            {
                playerName = PlayerInformation.instance.playerName,
                aquiredCharacters = PlayerInformation.instance.characterManager.aquiredCharacters,
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            PlayerInformation.instance.SetPlayerName(saveData.playerName);
            for (int i = 0; i < saveData.aquiredCharacters.Count; i++)
            {
                PlayerInformation.instance.characterManager.AddCharacter(saveData.aquiredCharacters[i]);
            }
        }

        [Serializable]
        private struct SaveData
        {
            public string playerName;
            public List<string> aquiredCharacters;
        }
    }

}