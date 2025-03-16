using Klaxon.GOAD;
using SerializableTypes;
using System;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class BallPeopleTravellerHomeSaveSystem : MonoBehaviour, ISaveable
	{
		public GOAD_Scheduler_BP travellerAI;

        public object CaptureState()
        {


            return new SaveData
            {
                travellerHome = travellerAI.BPHomeDestination
                
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            travellerAI.BPHomeDestination = saveData.travellerHome;
            
        }

        [Serializable]
        private struct SaveData
        {

            public SVector3 travellerHome;

            
        }
    }

}