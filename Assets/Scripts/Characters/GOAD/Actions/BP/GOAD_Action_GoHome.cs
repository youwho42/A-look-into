using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_GoHome : GOAD_Action
    {

        Vector2 offset;
        Vector2 lastDestination;

        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.animator.SetBool(agent.walking_hash, true);
            offset = new Vector2(Random.Range(-0.1f, 0.1f), -0.15f);
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (lastDestination == agent.walker.currentDestination)
                {
                    offset = new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
                }
                lastDestination = agent.walker.currentDestination;

                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }

            agent.walker.hasDeviatePosition = false;



            agent.walker.SetWorldDestination(agent.BPHomeDestination + (Vector3)offset);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {
                success = true;
                agent.SetActionComplete(true);
            }
            agent.walker.SetLastPosition();
        }

    }
}
