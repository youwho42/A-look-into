using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    public class FSM_FollowNodePathAction : FSM_Action
    {
        static int isGrounded_hash = Animator.StringToHash("IsGrounded");
        static int velocityX_hash = Animator.StringToHash("VelocityX");
        static int velocityY_hash = Animator.StringToHash("VelocityY");

        List<NavigationNode> path = new List<NavigationNode>();
        int currentPathIndex;
        NavigationNode currentNode;
        NavigationNode finalNavigationNode;

        public override void ExecuteAction(FSM_Brain brain)
        {
            Animator animator = brain.FSM_GetComponent<Animator>();
            animator.SetFloat(velocityX_hash, 1);
            GravityItemWalker walker = brain.FSM_GetComponent<GravityItemWalker>();

        }

        public override void LateExecuteAction(FSM_Brain brain)
        {
            
        }

        public override void ResetAction(FSM_Brain brain)
        {
            
        }
    }

}