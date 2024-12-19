using Klaxon.Interactable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class RobotPostSaveSystem : MonoBehaviour, ISaveable
    {
        
        public InteractableRobotBase robotBase;

        public object CaptureState()
        {

            return new SaveData
            {
                hasRobot = robotBase.hasRobot
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            robotBase.hasRobot = saveData.hasRobot;
            robotBase.canInteract = !robotBase.hasRobot;
        }
        

        [Serializable]
        private struct SaveData
        {
            public bool hasRobot;
        }
    }
}

