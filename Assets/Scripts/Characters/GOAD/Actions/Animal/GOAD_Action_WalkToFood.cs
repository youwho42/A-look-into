using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_WalkToFood : GOAD_Action
    {
        
        Vector3 eatPosition;

        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);

            if (agent.walker != null)
                agent.walker.enabled = true;

            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }
            if (agent.jumper != null)
            {
                if (agent.jumper.enabled)
                    agent.walker.facingRight = agent.jumper.facingRight;
                agent.jumper.enabled = false;
            }
            if (agent.currentEdible == null || agent.walker.itemObject.localPosition.z > 0)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            SetEatPosition(agent);
            agent.walker.SetWorldDestination(eatPosition);
            
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);
            if(agent.currentEdible == null || agent.walker.itemObject.localPosition.z > 0)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            if (NumberFunctions.GetDistanceV2(transform.position, eatPosition) <= 0.0004f)
            {
                agent.walker.currentDirection = Vector2.zero;
                if (transform.position.x < agent.currentEdible.transform.position.x && !agent.walker.facingRight)
                    agent.walker.Flip();
                else if (transform.position.x > agent.currentEdible.transform.position.x && agent.walker.facingRight)
                    agent.walker.Flip();

                agent.transform.position = eatPosition;
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {

                if (!agent.walker.jumpAhead)
                {
                    agent.DeviateWalk();
                    return;

                }
            }

            agent.walker.SetWorldDestination(eatPosition);
            agent.animator.SetBool(agent.walking_hash, true);
            agent.walker.SetDirection();

            agent.walker.SetLastPosition();


        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
        }
        

        void SetEatPosition(GOAD_Scheduler_Animal agent)
        {
            eatPosition = new Vector3(
                agent.transform.position.x <= agent.currentEdible.transform.position.x ? agent.currentEdible.transform.position.x - agent.xOffset : agent.currentEdible.transform.position.x + agent.xOffset,
                agent.currentEdible.transform.position.y - agent.eatPoint.localPosition.y,
                agent.currentEdible.transform.position.z);
        }
    }

}