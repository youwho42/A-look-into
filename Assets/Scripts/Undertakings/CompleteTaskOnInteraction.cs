
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public struct CompleteTaskObject
    {
        public UndertakingObject undertaking;
        public UndertakingTaskObject task;
    }
    public class CompleteTaskOnInteraction : MonoBehaviour
    {

        public CompleteTaskObject task;
        

        public void CompleteTask()
        {
            task.undertaking.TryCompleteTask(task.task);
        }
    }
}
