using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public class UndertakingDatabase : MonoBehaviour
    {


        public static UndertakingDatabase instance;
        private void Awake()
        {
            if (instance == null)
                instance = this;

            AddAllQuestsToDatabase();
            ResetQuests();
        }

        public List<UndertakingObject> allUndertakingObjects = new List<UndertakingObject>();

        public UndertakingObject GetUndertaking(string undertakingName)
        {
            foreach (var undertaking in allUndertakingObjects)
            {
                if(undertaking.Name == undertakingName)
                    return undertaking;
            }
            return null;
        }

        public void SetUndertakingState(UndertakingObject undertaking, int stateIndex)
        {
            if (allUndertakingObjects.Contains(undertaking))
                undertaking.CurrentState = (UndertakingState)stateIndex;
        }

        public void SetTaskState(string undertakingName, string taskName, bool state)
        {
            var q = GetUndertaking(undertakingName);
            if (allUndertakingObjects.Contains(q))
            {
                foreach (var task in q.Tasks)
                {
                    if(task.Name == taskName) 
                        task.IsComplete = state;
                }
            }
        }



        void AddAllQuestsToDatabase()
        {
            DirectoryInfo dataFolder = new DirectoryInfo("Assets/Resources/Undertakings");
            var folders = dataFolder.GetDirectories();
            foreach (var folder in folders)
            {
                string path = $"Undertakings/{folder.Name}";
                var obj = Resources.LoadAll<UndertakingObject>(path).ToList();
                allUndertakingObjects.AddRange(obj);
            }
        }

        public void ResetQuests()
        {
            for (int i = 0; i < allUndertakingObjects.Count; i++)
            {
                allUndertakingObjects[i].ResetUndertaking();
            }
        }
    }
}

