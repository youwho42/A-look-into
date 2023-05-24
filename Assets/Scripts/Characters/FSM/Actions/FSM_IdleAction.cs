using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    [CreateAssetMenu(menuName = "FSM/IdleAction")]
    public class FSM_IdleAction : FSM_Action
    {
        static int isGrounded_hash = Animator.StringToHash("IsGrounded");
        static int velocityX_hash = Animator.StringToHash("VelocityX");
        static int velocityY_hash = Animator.StringToHash("VelocityY");

        public override void ExecuteAction(FSM_Brain brain)
        {
            Animator animator = brain.FSM_GetComponent<Animator>();
            animator.SetFloat(velocityX_hash, 0);

            GravityItemWalker walker = brain.FSM_GetComponent<GravityItemWalker>();
            walker.currentDir = Vector2.zero;
        }

        public override void ResetAction(FSM_Brain brain)
        {
            
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