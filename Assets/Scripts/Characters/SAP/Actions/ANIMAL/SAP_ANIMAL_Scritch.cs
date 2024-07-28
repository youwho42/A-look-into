using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_ANIMAL_Scritch : SAP_Action
    {

        TreeRustling rustling;
        float timer;
        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            var dir = agent.currentScritchableItem.position - agent.currentScritchablePosition.position;
            if (dir.x > 0 && agent.walker.facingRight || dir.x < 0 && !agent.walker.facingRight)
                agent.walker.Flip();
            if (agent.currentScritchableItem.TryGetComponent(out TreeRustling treeRustling))
                rustling = treeRustling;
            transform.position = agent.currentScritchablePosition.position;
            agent.walker.SetLastPosition();
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isScritching_hash, true);

        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {


            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isScritching_hash, true);

            if (timer < 1)
                timer += Time.deltaTime;
            else
            {
                timer = 0;
                rustling.Affect(true);

            }


        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            
            agent.currentScritchableItem = null;
            agent.currentScritchablePosition = null;
            agent.scritchTimer = agent.scritchCoolDown;
            agent.animator.SetBool(agent.isScritching_hash, false);
            agent.SetBeliefState("Scritching", false);
            agent.isScritching = false;
        }


        
    }

} 
