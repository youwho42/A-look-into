using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_FleeJumper : GOAD_Action
    {
        public float fleeDistance;
       
        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);

            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isRunning_hash, true);


            if (agent.jumper != null)
                agent.jumper.enabled = true;
            if (agent.walker != null)
            {
                if (agent.walker.enabled)
                    agent.jumper.facingRight = agent.walker.facingRight;
                
                agent.walker.enabled = false;
            }


            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    if (agent.walker != null)
                        agent.jumper.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }

            agent.jumper.SetWorldDestination(FleeDestination(agent));
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);

            
            agent.animator.SetBool(agent.isRunning_hash, true);


            if (agent.jumper.isStuck)
            {

                agent.jumper.SetRandomDestination(fleeDistance);
                agent.jumper.isStuck = false;

            }

            agent.jumper.SetWorldDestination(agent.jumper.currentDestination);
            
            agent.jumper.SetDirection();

            agent.jumper.SetLastPosition();

            if (NumberFunctions.GetDistanceV2(transform.position, agent.jumper.currentDestination) <= GlobalSettings.DistanceCheck)
            {
                Debug.Log("Successfull flee");
                success = true;
                agent.SetActionComplete(true);
            }

            
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
            agent.animator.SetBool(agent.isRunning_hash, false);
        }

        public Vector2 FleeDestination(GOAD_Scheduler_Animal agent)
        {

            var allTiles = GridManager.instance.GetAllNearbyValidWorldPositions(agent.jumper.currentTilePosition.position, fleeDistance);

            Vector2 directionFlee = (Vector2)transform.position - (Vector2)PlayerInformation.instance.player.position;
            directionFlee = directionFlee.normalized;


            float bestScore = float.NegativeInfinity;
            Vector2 bestPosition = Vector3.negativeInfinity;
            foreach (var tile in allTiles)
            {
                var tileDir = (Vector2)transform.position - (Vector2)tile;
                tileDir = tileDir.normalized;
                var dist = NumberFunctions.GetDistanceV2((Vector2)transform.position, tile);
                dist /= fleeDistance * 2;
                var dot = Vector2.Dot(directionFlee, tileDir);
                dot = NumberFunctions.RemapNumber(dot, -1.0f, 1.0f, 1.0f, 0.0f);
                float result = dot + dist*.5f;
                if (result > bestScore)
                {
                    bestScore = result;
                    bestPosition = tile;
                }
                    
            }


            //Vector2 destination = (Vector2)transform.position + (directionFlee * fleeDistance);

            //return destination;
            return bestPosition;
        }

    }
}
