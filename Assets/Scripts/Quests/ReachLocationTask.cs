using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public class ReachLocationTask : MonoBehaviour
    {

        public UndertakingObject undertaking;
        public UndertakingTaskObject task;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (undertaking.CurrentState != UndertakingState.Active)
                return;
            if (collision.CompareTag("Player"))
                undertaking.TryCompleteTask(task);
        }
    }
}
