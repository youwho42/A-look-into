using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action_Wander : SAP_Action
    {
        List<NavigationNode> path = new List<NavigationNode>();
        int currentPathIndex;
        NavigationNode currentNode;
        

        bool isDeviating;
        bool finalDestination;

        public float wanderDistance = 1f;
        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            if (target == null)
                target = NavigationNodesManager.instance.GetRandomNode(NavigationNodeType.Outside, agent.pathType, transform.position, 3f);


            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);

            // Set the destination (currentAction.target) and direction here using currentAction.walker
            if (currentNode == null && path.Count <= 0)
            {
                path.Clear();
                currentPathIndex = 0;
                if (agent.lastValidNode != null)
                    currentNode = agent.lastValidNode;
                else
                    currentNode = NavigationNodesManager.instance.GetClosestNavigationNode(transform.position, agent.currentNavigationNodeType, agent.pathType);

                path = currentNode.FindPath(target);
                agent.walker.currentDestination = path[currentPathIndex].transform.position;
            }
        }
        public override void PerformAction(SAP_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.offScreen)
            {
                HandleOffScreen(agent);
                return;
            }

            if (agent.walker.isStuck || isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    if (finalDestination)
                    {
                        ReachFinalDestination(agent);
                        return;
                    }

                    Deviate(agent);
                    return;
                }
            }



            if (path.Count > 0)
            {
                agent.walker.currentDestination = path[currentPathIndex].transform.position;
            }
            
            //if (!walker.onSlope)
            agent.walker.SetDirection();
            
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {
                if (!finalDestination)
                {
                    if (currentPathIndex < path.Count - 1)
                    {
                        currentPathIndex++;
                        currentNode = path[currentPathIndex];
                        agent.walker.currentDestination = path[currentPathIndex].transform.position;
                    }
                    else if (currentPathIndex >= path.Count - 1)
                    {
                        agent.lastValidNode = currentNode;
                        
                        finalDestination = true;
                        agent.walker.SetRandomDestination(wanderDistance);
                    }
                }
                else
                {
                    ReachFinalDestination(agent);
                }

            }

            agent.walker.SetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_NPC agent)
        {
            agent.lastValidNode = currentNode;
            ReachFinalDestination(agent);
            currentNode = null;
            path.Clear();
            target = null;
        }

        private void HandleOffScreen(SAP_Scheduler_NPC agent)
        {
            agent.walker.currentDir = Vector2.zero;
            int frameSkip = 60;
            if (currentPathIndex < path.Count - 1)
            {
                var dist = (int)Vector2.Distance(path[currentPathIndex].transform.position, path[currentPathIndex + 1].transform.position) + 1;
                frameSkip *= dist;
            }
            if (Time.frameCount % frameSkip == 0)
            {
                agent.walker.transform.position = path[currentPathIndex].transform.position;
                agent.walker.currentTilePosition.position = agent.walker.currentTilePosition.GetCurrentTilePosition(agent.walker.transform.position);
                agent.walker.currentLevel = agent.walker.currentTilePosition.position.z;
                if (currentPathIndex < path.Count - 1)
                {
                    currentPathIndex++;
                    currentNode = path[currentPathIndex];
                }
                if (currentPathIndex >= path.Count - 1)
                {
                    agent.lastValidNode = currentNode;
                    
                    ReachFinalDestination(agent);
                }

            }
        }



        void Deviate(SAP_Scheduler_NPC agent)
        {
            isDeviating = true;
            if (agent.walker.isStuck)
                agent.walker.hasDeviatePosition = false;

            if (!agent.walker.hasDeviatePosition)
                agent.walker.FindDeviateDestination(agent.walker.tilemapObstacle ? 20 : 50);

            agent.animator.SetFloat(agent.velocityX_hash, 1);
            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            agent.walker.SetLastPosition();
        }

        void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {

            isDeviating = false;
            finalDestination = false;
            agent.currentGoalComplete = true;
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDir = Vector2.zero;
        }

    }
}
