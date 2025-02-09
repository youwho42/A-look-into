using System;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class JunkPileSaveSystem : MonoBehaviour, ISaveable
    {
        public JunkPileInteractor junkPileInteractor;

        public object CaptureState()
        {
            return new SaveData
            {
                hasInteracted = junkPileInteractor.hasInteracted
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            if (saveData.hasInteracted)
                junkPileInteractor.DisableGameObject();

        }

        [Serializable]
        private struct SaveData
        {
            public bool hasInteracted;

        }
    } 
}

