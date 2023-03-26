using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Klaxon.UndertakingSystem
{
    [CreateAssetMenu(menuName = "Undertakings/Undertaking Database")]
    public class UndertakingDatabase : ScriptableObject
    {

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
        public UndertakingTaskObject GetTask(string undertakingName, string taskName)
        {
            foreach (var undertaking in allUndertakingObjects)
            {
                if (undertaking.Name == undertakingName)
                {
                    foreach (var task in undertaking.Tasks)
                    {
                        if(task.Name == taskName) 
                            return task;
                    }
                }
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

        public void ResetUndertakings()
        {
            for (int i = 0; i < allUndertakingObjects.Count; i++)
            {
                allUndertakingObjects[i].ResetUndertaking();
            }
        }
    }
}

