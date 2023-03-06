
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public class CompleteTaskOnInteraction : MonoBehaviour
    {
        public UndertakingObject undertaking;
        public UndertakingTaskObject task;

        public void CompleteTask()
        {
            undertaking.TryCompleteTask(task);
        }
    }
}
