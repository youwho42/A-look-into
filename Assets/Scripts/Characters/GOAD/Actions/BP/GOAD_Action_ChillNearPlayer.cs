using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_ChillNearPlayer : GOAD_Action
	{

        float timeIdle;
        bool sleeping;


        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.interactor.canInteract = true;

            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.walker.ResetLastPosition();
            

        }
        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);

            if (!agent.CheckNearPlayer(sleeping ? 0.85f : 0.5f))
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            if (sleeping)
            {
                agent.interactor.canInteract = false;
                agent.animator.SetBool(agent.sleeping_hash, true);
                return;
            }

            var dir = PlayerInformation.instance.player.position - transform.position;
            agent.walker.SetFacingDirection(dir);
            agent.arms.SetActive(!agent.hasInteracted);
            agent.interactor.canInteract = !agent.hasInteracted;

            timeIdle += Time.deltaTime;

            
            if (timeIdle >= 10.0f)
                sleeping = true;
        }

        public override void SucceedAction(GOAD_Scheduler_BP agent)
        {
            base.SucceedAction(agent);
        }

        public override void FailAction(GOAD_Scheduler_BP agent)
        {
            base.FailAction(agent);
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            timeIdle = 0;
            sleeping = false;
            agent.animator.SetBool(agent.sleeping_hash, false);
            
        }
    } 
}
