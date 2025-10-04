using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_GrandmaFuneral : GOAD_Action
    {
        bool atGrave;
        public Transform finalNode;
        Vector3 finalPosition;
        public GOAD_ScriptableCondition funeralOverCondition;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            finalPosition = GetPositionAroundGrave();
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);
            if (atGrave)
            {
                
                if (agent.IsConditionMet(funeralOverCondition))
                {
                    success = true;
                    agent.SetActionComplete(true);
                }
                return;
            }
            if (!atGrave)
            {
                agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                agent.animator.SetFloat(agent.velocityX_hash, 1);

                agent.walker.currentDestination = finalPosition;

                agent.walker.SetDirection();
                if (agent.walker.CheckDistanceToDestination() <= GlobalSettings.DistanceCheck)
                {
                    agent.animator.SetFloat(agent.velocityX_hash, 0);
                    agent.walker.currentDirection = Vector2.zero;
                    atGrave= true;
                } 
            }
        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
        }

        Vector3 GetPositionAroundGrave()
        {
            Vector3 position = Vector3.zero;
            bool found = false;
            do
            {
                var r = Random.insideUnitCircle * .4f;
                var pos = finalNode.position + (Vector3)r;
                var hit = Physics2D.OverlapPoint(pos, LayerMask.GetMask("Obstacle"));
                if(hit == null)
                {
                    position = pos;
                    found = true;
                }
            } while (!found);
            return position;
        }
    }
}
