using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    [CreateAssetMenu(menuName = "Undertakings/Task")]
    public class UndertakingTaskObject : ScriptableObject
    {
        public string Name;
        public bool IsComplete;

        public void CompleteTask()
        {
            IsComplete = true;
        }

        public void ResetTask()
        {
            IsComplete = false;
        }
    }
}

