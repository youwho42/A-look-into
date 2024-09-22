using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_AnimalRest : GOAD_Action
    {
        public Vector2 minMaxRestTime;
        float restTimer;
        float headTimer;
        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);

            restTimer = agent.SetRandomRange(minMaxRestTime);

            if (agent.walker != null)
            {
                agent.walker.enabled = true;
                agent.walker.currentDirection = Vector2.zero;
            }


            if (agent.flier != null)
            {
                if (agent.walker != null)
                {
                    if (agent.walker.itemObject.localPosition.y != 0)
                        agent.walker.isWeightless = true;
                }
                if (agent.flier.enabled)
                {
                    if (agent.walker != null)
                        agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }
            if (agent.jumper != null)
            {
                if (agent.jumper.enabled && agent.walker != null)
                    agent.walker.facingRight = agent.jumper.facingRight;
                agent.jumper.enabled = false;
            }

            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isSitting_hash, true);
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);
            restTimer -= Time.deltaTime;

            headTimer -= Time.deltaTime;
            if (headTimer <= 0)
                headTimer = agent.TurnHead();


            if (restTimer <= 0)
            {
                success = true;
                agent.SetActionComplete(true);
            }
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
            agent.animator.SetBool(agent.isSitting_hash, false);
        }
        
    }
}
