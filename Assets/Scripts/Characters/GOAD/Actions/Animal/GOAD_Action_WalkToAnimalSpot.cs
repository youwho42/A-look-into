using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_WalkToAnimalSpot : GOAD_Action
    {
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

            if (agent.currentDisplacementSpot == null)
                agent.currentDisplacementSpot = agent.CheckForDisplacementSpot();
            if (agent.currentDisplacementSpot == null)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            

            agent.walker.SetWorldDestination(agent.currentDisplacementSpot.transform.position);
            agent.animator.SetBool(agent.walking_hash, true);

            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false;
            }
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);
            if (agent.currentDisplacementSpot == null)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }


            if (Vector2.Distance(transform.position, agent.currentDisplacementSpot.transform.position) <= 0.02f)
            {
                agent.transform.position = agent.currentDisplacementSpot.transform.position;
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

            agent.walker.SetWorldDestination(agent.currentDisplacementSpot.transform.position);
            agent.animator.SetBool(agent.walking_hash, true);
            agent.walker.SetDirection();

            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
            agent.walker.currentDirection = Vector2.zero;
            agent.closestSpots.Clear();

            //if (agent.currentDisplacementSpot != null)
            //    agent.currentDisplacementSpot.isInUse = true;

        }

    }

}