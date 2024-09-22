using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_Land : GOAD_Action
    {
        float timer;

        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);

            if (agent.flier != null)
                agent.flier.enabled = true;

            if (agent.walker != null)
            {
                if (agent.walker.enabled)
                {
                    agent.flier.facingRight = agent.walker.facingRight;
                    agent.walker.enabled = false;
                }
            }

            agent.currentDisplacementSpot = agent.CheckForDisplacementSpot();
            if (agent.currentDisplacementSpot == null)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            agent.flier.SetDestination(agent.currentDisplacementSpot);
            agent.flier.isLanding = true;

            timer = agent.SetRandomRange(agent.minMaxFlap);

            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.landed_hash, false);

            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false;
            }


            agent.SetBoidsState(false);
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);

            //if (agent.sleep.isSleeping)
            //{
            //    agent.HandleOffScreen(this);
            //    return;
            //}

            if (agent.currentDisplacementSpot == null)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }



            if (agent.currentDisplacementSpot.positionZ == 0)
            {
                if (agent.flier.itemObject.localPosition.y <= 0.03f)
                {
                    agent.flier.itemObject.localPosition = Vector3.zero;
                    success = true;
                    agent.SetActionComplete(true);
                    return;
                }
            }
            else
            {
                if (Vector3.Distance(agent.flier.itemObject.localPosition, agent.currentDisplacementSpot.displacedPosition) <= 0.05f && Vector2.Distance(agent.transform.position, agent.currentDisplacementSpot.transform.position) <= 0.05f)
                {
                    agent.flier.itemObject.localPosition = agent.currentDisplacementSpot.displacedPosition;
                    agent.transform.position = agent.currentDisplacementSpot.transform.position;
                    success = false;
                    agent.SetActionComplete(true);
                    return;
                }
            }



            if (agent.flier.isStuck || agent.isDeviating || !agent.flier.canReachNextTile)
            {
                agent.DeviateFly();
                return;
            }



            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                agent.glide = !agent.glide;
                timer = agent.glide ? agent.SetRandomRange(agent.minMaxGlide) : agent.SetRandomRange(agent.minMaxFlap);

                agent.animator.SetBool(agent.gliding_hash, agent.glide);
            }

            agent.flier.SetDestination(agent.currentDisplacementSpot);


            
            

            agent.flier.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
            agent.flier.isLanding = false;
            agent.glide = false;
            agent.closestSpots.Clear();
            if (agent.currentDisplacementSpot != null)
                agent.currentDisplacementSpot.isInUse = true;
                
        }

    }
}
