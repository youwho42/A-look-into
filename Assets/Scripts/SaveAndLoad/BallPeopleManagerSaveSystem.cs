using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class BallPeopleManagerSaveSystem : MonoBehaviour, ISaveable
    {
        public BallPeopleManager ballPeopleManager;
        public object CaptureState()
        {
            List<int> accessoryQueue = new List<int>();
            foreach (var item in ballPeopleManager.accessoryIndexQueue)
            {
                accessoryQueue.Add(item);
            }
            return new SaveData
            {
                accessoryIndexQueue = accessoryQueue
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            foreach (var item in saveData.accessoryIndexQueue)
            {
                ballPeopleManager.accessoryIndexQueue.Enqueue(item);
            }


        }

        [Serializable]
        private struct SaveData
        {
            public List<int> accessoryIndexQueue;
        }
    } 
}
