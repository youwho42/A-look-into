using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class BallPeopleColorAccessorySaveSystem : MonoBehaviour, ISaveable
    {

        public RandomAccessories accessories;
        public RandomColor colors;
        
        public object CaptureState()
        {

            
            return new SaveData
            {
                accessoryIndex = accessories.accessoryIndex,
                r = colors.randomColor.r,
                g = colors.randomColor.g,
                b = colors.randomColor.b,
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            colors.SetColor(saveData.r, saveData.g, saveData.b);
            accessories.PopulateList();
            accessories.SetAccessories(saveData.accessoryIndex);
            

        }

        [Serializable]
        private struct SaveData
        {

            public int accessoryIndex;
            public float r;
            public float g;
            public float b;

        }
    } 
}
