using UnityEngine;
using System.Collections.Generic;


namespace Klaxon.GOAD
{
    public class GOAD_Action_SwimWander : GOAD_Action
    {
        public bool useStableZ;
        public bool temporarySwimming;
        [ConditionalHide("temporarySwimming", true)]
        public Vector2 minMaxTimeToSwim;
        float timer;
        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);
            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    agent.swimmer.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }

            timer = Random.Range(minMaxTimeToSwim.x, minMaxTimeToSwim.y);
            if (agent.walker != null)
            {
                if (agent.walker.enabled)
                {
                    agent.swimmer.facingRight = agent.walker.facingRight;
                    agent.walker.enabled = false;
                }
            }
            agent.swimmer.enabled = true;
            agent.swimmer.SetRandomDestination();
            agent.animator.SetBool("Swimming", true);
            float randomIdleStart = Random.Range(0, agent.animator.GetCurrentAnimatorStateInfo(0).length);
            agent.animator.Play(0, 0, randomIdleStart);
            
            //agent.animator.SetBool(agent.landed_hash, false);
            //agent.animator.SetBool(agent.gliding_hash, false);
            //agent.animator.SetBool(agent.sleeping_hash, false);
            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false;
            }
            if(useStableZ)
            {
                var pos = new Vector3(0, agent.swimmer.minMaxSwimZ.x * GlobalSettings.SpriteDisplacementY, agent.swimmer.minMaxSwimZ.x);
                agent.swimmer.itemObject.localPosition = pos;
            }
            agent.SetBoidsState(true);
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);
            if(temporarySwimming)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    success = true;
                    agent.SetActionComplete(true);
                    return;
                }
            }
            if (!agent.swimmer.boid.inBoidPool)
            {
                if (Vector2.Distance(agent.swimmer.transform.position, agent.swimmer.currentDestination) <= 0.03f/* || agent.swimmer.CheckDistanceToDestination() <= 0.02f*/)
                    agent.SetBoidsState(true);
            }

            agent.animator.SetBool("Swimming", true);
            //agent.SetBoidsState(true);

            if (!useStableZ)
            {
                if (Vector2.Distance(agent.swimmer.itemObject.localPosition, agent.swimmer.currentDestinationZ) <= 0.02f/* || agent.swimmer.CheckDistanceToDestination() <= 0.02f*/)
                    agent.swimmer.SetRandomDestinationZ();
            }

            if (!agent.swimmer.canReachNextTile)
            {
                agent.swimmer.boid.inBoidPool = false;
                agent.swimmer.SetRandomDestination();
            }
            agent.swimmer.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
            
            
            agent.animator.SetBool("Swimming", false);
            agent.flier.isWaterLanding = false;
        }
    }
}
