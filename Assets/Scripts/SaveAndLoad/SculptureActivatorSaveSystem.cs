using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class SculptureActivatorSaveSystem : MonoBehaviour, ISaveable
	{
        public SphereSuperShapeActivator shapeActivator;
        public object CaptureState()
        {
            return new SaveData
            {
                isActivated = shapeActivator.isActivated
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            shapeActivator.SetActivatorFromSave(saveData.isActivated);
        }

        [Serializable]
        private struct SaveData
        {

            public bool isActivated;
        }
    } 
}
