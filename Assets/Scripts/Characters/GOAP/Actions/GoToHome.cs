using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Klaxon.GOAP
{
    public class GoToHome : GOAP_Action
    {
        List<NavigationNode> path = new List<NavigationNode>();
        int currentPathIndex;
        NavigationNode currentNode;


        bool isDeviating;


        public override bool PrePerform(GOAP_Agent agent)
        {


            if (target != null)
            {

                // Set the destination (currentAction.target) and direction here using currentAction.walker
                if (currentNode == null && path.Count <= 0)
                {
                    path.Clear();
                    currentPathIndex = 0;
                    currentNode = NavigationNodesManager.instance.GetClosestNavigationNode(transform.position, agent.currentNavigationNodeType);
                    path = currentNode.FindPath(target);
                    walker.currentDestination = path[currentPathIndex].transform.position;
                }


            }
            return true;
        }

        public override void Perform(GOAP_Agent agent)
        {

            agent.animator.SetBool(agent.isGrounded_hash, walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);
            // this is where we need to make the npc GO TO the destination.
            // use currentAction.walker here


            agent.animator.SetFloat(agent.velocityX_hash, 1);



            if (walker.isStuck || isDeviating)
            {
                if (!walker.jumpAhead)
                {
                    Deviate(agent);
                    return;
                }
            }



            if (path.Count > 0)
            {
                walker.currentDestination = path[currentPathIndex].transform.position;
            }
            if (!walker.onSlope)
                walker.SetDirection();
            if (walker.CheckDistanceToDestination() <= 0.02f)
            {
                if (currentPathIndex < path.Count - 1)
                {
                    currentPathIndex++;
                    walker.currentDestination = path[currentPathIndex].transform.position;
                }
                else if (currentPathIndex == path.Count - 1)
                {
                    path.Clear();
                    currentPathIndex = 0;
                    currentNode = null;
                    agent.destinationReached = true;
                    agent.animator.SetFloat(agent.velocityX_hash, 0);
                    walker.currentDir = Vector2.zero;
                }
            }

            walker.SetLastPosition();



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

        public override void PrePostPerform(GOAP_Agent agent)
        {
            
        }

        public override bool PostPerform(GOAP_Agent agent)
        {
            isDeviating = false;
            currentNode = null;
            path.Clear();
            agent.beliefs.RemoveState("NotHome");
            return true;
        }


    } 
}
