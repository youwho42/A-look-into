using Klaxon.GOAD;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class RobotSaveSystem : MonoBehaviour, ISaveable
    {
        public GOAD_Scheduler_Robot robot;
        

        public object CaptureState()
        {

            return new SaveData
            {
                homeBasePosition = robot.homeBase
            };
        }

        public void RestoreState(object state)
        {
            StartCoroutine(RestoreStateCo(state));
        }
        public IEnumerator RestoreStateCo(object state)
        {
            var saveData = (SaveData)state;
            yield return new WaitForSeconds(0.35f);
            robot.homeBase = saveData.homeBasePosition;
            
        }

        [Serializable]
        private struct SaveData
        {
            public SVector3 homeBasePosition;
           
        }
    }

}