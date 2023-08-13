using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Klaxon.SaveSystem
{
    public class PlayerSpriteSaveSystem : MonoBehaviour, ISaveable
    {
        public SpriteResolver spriteResolver;
        public object CaptureState()
        {
            return new SaveData
            {
                spriteName = spriteResolver.GetLabel()
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            spriteResolver.SetCategoryAndLabel("Player", saveData.spriteName);

        }

        [Serializable]
        private struct SaveData
        {
            public string spriteName;

        }
    }

}