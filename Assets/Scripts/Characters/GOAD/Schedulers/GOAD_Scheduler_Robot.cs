using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Scheduler_Robot : GOAD_Scheduler
	{
        public readonly int isGrounded_hash = Animator.StringToHash("Direction");
        public readonly int isRunning_hash = Animator.StringToHash("Gather");
        public readonly int isSitting_hash = Animator.StringToHash("Wander");
        public readonly int isIdleSitting_hash = Animator.StringToHash("Open");
        public readonly int isSleeping_hash = Animator.StringToHash("Close");
        public readonly int isCrafting_hash = Animator.StringToHash("Deactivate");
        public Animator animator;

    } 
}
