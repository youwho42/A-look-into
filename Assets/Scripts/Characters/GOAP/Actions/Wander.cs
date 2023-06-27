using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAP
{
    public class Wander : GOAP_Action
    {
        List<NavigationNode> path = new List<NavigationNode>();
        int currentPathIndex;
        NavigationNode currentNode;
        NavigationNode lastValidNode;

        bool isDeviating;
        bool findSeedPosition;

        public override bool PrePerform(GOAP_Agent agent)
        {
            if (target == null)
                target = NavigationNodesManager.instance.GetRandomNode(NavigationNodeType.Outside, agent.pathType,transform.position, 5f);


            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);

            // Set the destination (currentAction.target) and direction here using currentAction.walker
            if (currentNode == null && path.Count <= 0)
            {
                path.Clear();
                currentPathIndex = 0;
                if(lastValidNode != null)
                    currentNode = lastValidNode;
                else
                    currentNode = NavigationNodesManager.instance.GetClosestNavigationNode(transform.position, agent.currentNavigationNodeType, agent.pathType);

                path = currentNode.FindPath(target);
                walker.currentDestination = path[currentPathIndex].transform.position;
            }

            agent.beliefs.SetState("NotHome", 0);
            return true;
        }

        public override void Perform(GOAP_Agent agent)
        {
            agent.animator.SetBool(agent.isGrounded_hash, walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);



            if (walker.isStuck || isDeviating)
            {
                if (!walker.jumpAhead)
                {
                    if (findSeedPosition)
                    {
                        ReachFinalDestinaion(agent);
                        return;
                    }

                    Deviate(agent);
                    return;
                }
            }



            if (path.Count > 0)
            {
                walker.currentDestination = path[currentPathIndex].transform.position;
            }
            if (agent.offScreen)
            {
                HandleOffScreen(agent);
                return;
            }
            //if (!walker.onSlope)
            walker.SetDirection();
            if (walker.CheckDistanceToDestination() <= 0.02f)
            {
                if (!findSeedPosition)
                {
                    if (currentPathIndex < path.Count - 1)
                    {
                        currentPathIndex++;
                        walker.currentDestination = path[currentPathIndex].transform.position;
                    }
                    else if (currentPathIndex == path.Count - 1)
                    {
                        lastValidNode = path[currentPathIndex];
                        path.Clear();
                        currentPathIndex = 0;
                        currentNode = null;
                        findSeedPosition = true;
                        walker.SetRandomDestination(agent.wanderDistance);
                    }
                }
                else
                {
                    ReachFinalDestinaion(agent);
                }

            }

            walker.SetLastPosition();
        }

        

        public override void PrePostPerform(GOAP_Agent agent)
        {

        }

        public override bool PostPerform(GOAP_Agent agent)
        {

            target = null;
            return true;
        }

        private void HandleOffScreen(GOAP_Agent agent)
        {
            walker.currentDir = Vector2.zero;
            int frameSkip = 60;
            if (currentPathIndex < path.Count - 1)
            {
                var dist = (int)Vector2.Distance(path[currentPathIndex].transform.position, path[currentPathIndex + 1].transform.position) + 1;
                frameSkip *= dist;
            }
            if (Time.frameCount % frameSkip == 0)
            {
                walker.transform.position = path[currentPathIndex].transform.position;
                walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(walker.transform.position);
                if (currentPathIndex < path.Count - 1)
                    currentPathIndex++;
                if (currentPathIndex == path.Count - 1)
                {
                    lastValidNode = path[currentPathIndex];
                    path.Clear();
                    currentPathIndex = 0;
                    currentNode = null;
                    ReachFinalDestinaion(agent);
                }

            }
        }


        void ReachFinalDestinaion(GOAP_Agent agent)
        {

            isDeviating = false;
            findSeedPosition = false;
            agent.destinationReached = true;
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            walker.currentDir = Vector2.zero;
        }

        void Deviate(GOAP_Agent agent)
        {
            isDeviating = true;
            if (walker.isStuck)
                walker.hasDeviatePosition = false;

            if (!walker.hasDeviatePosition)
                walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);

            agent.animator.SetFloat(agent.velocityX_hash, 1);
            walker.SetDirection();

            if (walker.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            walker.SetLastPosition();
        }

    }
}

