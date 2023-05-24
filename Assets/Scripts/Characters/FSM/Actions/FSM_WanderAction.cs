using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    [CreateAssetMenu(menuName = "FSM/WanderAction")]
    public class FSM_WanderAction : FSM_Action
    {
        public float wanderDistance = 2;
        public bool hasDestination = false;

        static int isGrounded_hash = Animator.StringToHash("IsGrounded");
        static int velocityX_hash = Animator.StringToHash("VelocityX");
        static int velocityY_hash = Animator.StringToHash("VelocityY");

        public override void ExecuteAction(FSM_Brain brain)
        {
            Animator animator = brain.FSM_GetComponent<Animator>();
            animator.SetFloat(velocityX_hash, 1);

            GravityItemWalker walker = brain.FSM_GetComponent<GravityItemWalker>();
            if (!hasDestination)
            {
                walker.SetRandomDestination(wanderDistance);
                hasDestination = true;
            }
            
            walker.SetDirection();
            walker.SetLastPosition();
        }

        public override void ResetAction(FSM_Brain brain)
        {
            hasDestination = false;
        }

        public override void LateExecuteAction(FSM_Brain brain)
        {
            Animator animator = brain.FSM_GetComponent<Animator>();
            GravityItemWalker walker = brain.FSM_GetComponent<GravityItemWalker>();

            animator.SetBool(isGrounded_hash, walker.isGrounded);
            animator.SetFloat(velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);
        }
    }
}
