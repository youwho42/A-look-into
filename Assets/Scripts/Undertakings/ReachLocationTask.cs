using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public class ReachLocationTask : MonoBehaviour
    {

        public UndertakingObject undertaking;
        public UndertakingTaskObject task;
        public bool canSelfActivate;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
                return;

            if (canSelfActivate)
                undertaking.ActivateUndertaking();

            if (undertaking.CurrentState != UndertakingState.Active )
                return;

            
            undertaking.TryCompleteTask(task);
        }
    }
}
