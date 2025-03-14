using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_AStarToNode : GOAD_Action
    {

        bool hasResetFinalDestination;
        public override void StartAction(GOAD_Scheduler_Robot agent)
        {
            base.StartAction(agent);
            agent.animator.SetBool(agent.Wander_hash, true);
            //agent.animator.SetBool(agent.isSleeping_hash, false);
            if (agent.homeBase != null)
            {
                agent.currentPathIndex = 0;
                agent.aStarPath.Clear();
                if (agent.StartPositionValid())
                    agent.SetAStarDestination(agent.homeBase, this);

                agent.currentFinalDestination = agent.homeBase;

            }
            agent.robotLights.SetCurrentFunction(RobotLightManager.RobotStates.Retiring);
        }

        public override void PerformAction(GOAD_Scheduler_Robot agent)
        {
            base.PerformAction(agent);
            //agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            //agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            //agent.animator.SetFloat(agent.velocityX_hash, 1);
            if (agent.gettingPath)
                return;

            if(!hasResetFinalDestination)
            {
                agent.aStarPath[agent.aStarPath.Count - 1] = agent.homeBase;
                hasResetFinalDestination = true;
            }

            if(agent.HasBelief(agent.robotActiveCondition.Condition, true))
            {
                success = false;
                agent.SetActionComplete(true);
            }

            float dir = agent.animator.GetFloat(agent.Direction_hash);
            if (dir < 1)
                agent.animator.SetFloat(agent.Direction_hash, dir + Time.deltaTime);

            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenAStar(this);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {
                agent.AStarDeviate(this);
                return;
            }
            if (agent.aStarPath.Count == 0)
                agent.aStarPath.Add(agent.homeBase);

            if (agent.aStarPath.Count > 0)
                agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];

            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.02f)
            {
                agent.lastValidTileLocation = agent.aStarPath[agent.currentPathIndex];
                if (agent.currentPathIndex < agent.aStarPath.Count - 1)
                {
                    agent.currentPathIndex++;
                    agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];
                }
                else if (agent.currentPathIndex >= agent.aStarPath.Count - 1)
                {
                    success = true;
                    agent.SetActionComplete(true);
                }

                

            }


            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_Robot agent)
        {
            base.EndAction(agent);
            //agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
            agent.aStarPath.Clear();
            agent.currentPathIndex = 0;
            hasResetFinalDestination = false;
            agent.animator.SetBool(agent.Wander_hash, false);
        }
        public override void AStarDestinationIsCurrentPosition(GOAD_Scheduler_NPC agent)
        {
            success = true;
            agent.SetActionComplete(true);
        }

    }
}
