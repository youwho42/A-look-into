
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public class TalkToNPCTask : MonoBehaviour
    {
        public UndertakingObject undertaking;
        public UndertakingTaskObject task;

        public void TalkTask()
        {
            undertaking.TryCompleteTask(task);
        }
    }
}
