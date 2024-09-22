using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_Scritch : GOAD_Action
    {
        TreeRustling rustling;
        float timer;
        public Vector2 minMaxScritchTime;
        float scritchTimer;
        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);
            //var dir = agent.currentScritchableItem.position - agent.currentScritchablePosition.position;
            //if (dir.x > 0 && agent.walker.facingRight || dir.x < 0 && !agent.walker.facingRight)
            //    agent.walker.Flip();
            rustling = agent.currentDisplacementSpot.GetComponentInParent<TreeRustling>();
            if(rustling == null) 
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }
            var dir = agent.currentDisplacementSpot.transform.position - rustling.transform.position;
            if (dir.x < 0 && agent.walker.facingRight || dir.x > 0 && !agent.walker.facingRight)
                agent.walker.Flip();
            scritchTimer = agent.SetRandomRange(minMaxScritchTime);
            //transform.position = agent.currentScritchablePosition.position;
            agent.walker.SetLastPosition();
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isScritching_hash, true);
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);

            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isScritching_hash, true);

            if (timer < 1)
                timer += Time.deltaTime;
            else
            {
                timer = 0;
                rustling.Affect(true);

            }
            scritchTimer -= Time.deltaTime;
            if (scritchTimer <= 0)
            {
                success = true;
                agent.SetActionComplete(true);
            }
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);

            if (agent.currentDisplacementSpot != null)
                agent.currentDisplacementSpot = null;

            timer = 0;
            agent.animator.SetBool(agent.isScritching_hash, false);
            agent.isScritching = false;
        }
        
    }
}
