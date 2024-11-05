using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public class ReachLocationTask : MonoBehaviour
    {
        public CompleteTaskObject taskObject;
        
        public bool canSelfActivate;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
                return;

            if (collision.gameObject.transform.position.z != transform.position.z)
                return;
            
            if (canSelfActivate)
                taskObject.undertaking.ActivateUndertaking();

            if (taskObject.undertaking.CurrentState == UndertakingState.Active)
                taskObject.undertaking.TryCompleteTask(taskObject.task);
        }
    }
}
