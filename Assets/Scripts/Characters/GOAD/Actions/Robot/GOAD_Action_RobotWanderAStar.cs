using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_RobotWanderAStar : GOAD_Action
    {
        public int wanderDistance = 3;
        public override void StartAction(GOAD_Scheduler_Robot agent)
        {
            base.StartAction(agent);

            agent.offScreenPosMoved = true;

            //agent.animator.SetBool(agent.isSitting_hash, false);
            //agent.animator.SetBool(agent.isSleeping_hash, false);
            agent.animator.SetBool(agent.Wander_hash, true);
            agent.currentPathIndex = 0;
            agent.aStarPath.Clear();
            if (agent.StartPositionValid())
                agent.GetRandomTilePosition(wanderDistance, this);
            else
                agent.aStarPath.Add(agent.lastValidTileLocation);
            agent.robotLights.SetCurrentFunction(RobotLightManager.RobotStates.Roaming);
        }
        
        public override void PerformAction(GOAD_Scheduler_Robot agent)
        {
            base.PerformAction(agent);

            if (agent.gettingPath)
                return;
            if (agent.aStarPath.Count <= 0 || agent.HasBelief(agent.robotActiveCondition.Condition, false))
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            if (agent.currentPathIndex >= agent.aStarPath.Count)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
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

            if (agent.aStarPath.Count > 0)
                agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];

            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= GlobalSettings.DistanceCheck)
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
            agent.animator.SetBool(agent.Wander_hash, false);
            agent.walker.currentDirection = Vector2.zero;
            agent.aStarPath.Clear();
            agent.currentPathIndex = 0;
        }

    public override void AStarDestinationIsCurrentPosition(GOAD_Scheduler_Robot agent)
    {
        success = true;
        agent.SetActionComplete(true);

    }

}

}