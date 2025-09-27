using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_FleeFlier : GOAD_Action
    {

        public float fleeDistance = 1;

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

            if(agent.fleeTransform == null)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            agent.flier.currentDestination = GetFleeDestination(agent);
            agent.flier.SetRandomDestinationZ();
            agent.flier.SetDirection();

            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.landed_hash, false);
            agent.animator.SetBool(agent.gliding_hash, false);
            agent.animator.SetBool(agent.sleeping_hash, false);
            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false;
            }

        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);

            if (NumberFunctions.GetDistanceV2(agent.flier.itemObject.localPosition, agent.flier.currentDestinationZ) <= 0.0004f || agent.flier.CheckDistanceToDestination() <= 0.0004f)
            {
                success = true;
                agent.SetActionComplete(true);
            }

            if (agent.flier.isStuck || agent.isDeviating || !agent.flier.canReachNextTile)
            {
                agent.DeviateFly();
                return;
            }

            agent.flier.SetDirection();

            
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
            agent.fleeTransform = null;
        }

        Vector3 GetFleeDestination(GOAD_Scheduler_Animal agent)
        {
            Debug.Log("get flee direction");
            var dir = (transform.position - agent.fleeTransform.position).normalized * fleeDistance;
            dir.z = transform.position.z;
            return dir;
        }
    }

}