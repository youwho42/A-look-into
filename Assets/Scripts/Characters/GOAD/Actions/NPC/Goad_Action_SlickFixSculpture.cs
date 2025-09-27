using UnityEngine;

namespace Klaxon.GOAD
{
	public class Goad_Action_SlickFixSculpture : GOAD_Action
	{
        SlickSculpturesManager sculpturesManager;
        RestoreSculpture currentSculpture;
        bool isRestoring;
        bool atSculpture;
        bool atNode;
        bool goingToNode;
        bool isRestored;
        bool isLeaving;
        bool isDone;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            isRestoring = false;
            atSculpture = false;
            atNode = false;
            goingToNode = false;
            isRestored = false;
            isLeaving = false;
            isDone = false;
            sculpturesManager = SlickSculpturesManager.instance;
            currentSculpture = sculpturesManager.GetNextSculpture();
            if (currentSculpture == null)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            bool atFixNode = false;
            if (NumberFunctions.GetDistanceV2(transform.position, currentSculpture.fixNode.transform.position) <= 0.0004f)
            {
                atFixNode = true;
                atSculpture = true;
                atNode = true;
                goingToNode = true;
                agent.currentNode = currentSculpture.fixNode;
            }

            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);

            if (atFixNode)
                return;
            agent.currentPathIndex = 0;
            agent.aStarPath.Clear();
            if (agent.StartPositionValid())
                agent.SetAStarDestination(currentSculpture.reachNode.transform.position, this);

            agent.currentFinalDestination = currentSculpture.reachNode.transform.position;

           
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);
            if (agent.gettingPath)
                return;

            if(isDone)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            if (!atSculpture)
            {
                WalkToSculpture(agent);
                return;
            }
            if (atSculpture && !goingToNode)
            { 
                agent.currentNode = currentSculpture.reachNode;
                GetPathToNode(agent, currentSculpture.fixNode);
                goingToNode = true;
                return;
            }
            if(goingToNode && !atNode)
            {
                WalkToNode(agent);
                return;
            }

            if (isRestored && !isLeaving)
            {
                agent.currentNode = currentSculpture.fixNode;
                GetPathToNode(agent, currentSculpture.reachNode);
                isLeaving = true;
                return;
            }
            if (isLeaving)
            {
                WalkToNode(agent);
                return;
            }

            


            if (!isRestoring)
            {
                agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                agent.animator.SetFloat(agent.velocityX_hash, 0);
                agent.walker.currentDirection = Vector2.zero;
                GameEventManager.onTimeTickEvent.AddListener(Tick);
                if (agent.walker.facingRight && currentSculpture.fixNode.transform.position.x < currentSculpture.reachNode.transform.position.x
                    || !agent.walker.facingRight && currentSculpture.fixNode.transform.position.x > currentSculpture.reachNode.transform.position.x)
                    agent.walker.Flip();

                if (currentSculpture.ticks == 0)
                    currentSculpture.ticks = GetSculptureRestoreTicks();
                 
                currentSculpture.fixSounds.StartSoundsNoTimer();

                currentSculpture.interactablePainting.canInteract = false;
                agent.animator.SetBool(agent.isCrafting_hash, true);
                isRestoring = true;
                currentSculpture.fixParticles.Play();
            }

            if (currentSculpture.ticks <= 0)
            {
                currentSculpture.ticks = 0;
                GameEventManager.onTimeTickEvent.RemoveListener(Tick);
                agent.animator.SetBool(agent.isCrafting_hash, false);
                currentSculpture.interactablePainting.canInteract = true;
                SetSculptureIngredientsActive();
                currentSculpture.GetIsFinished();
                sculpturesManager.RemoveSculptureFromQueue();
                isRestored = true;
                GetPathToNode(agent, currentSculpture.reachNode);
                currentSculpture.fixParticles.Stop();
                currentSculpture.fixSounds.StopSoundsNoTimer();
            }
        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            currentSculpture = null;
        }

        void Tick(int tick)
        {
            currentSculpture.ticks--;
        }

        int GetSculptureRestoreTicks()
        {
            int ticks = 0;
            foreach (var item in currentSculpture.ingredients)
            {
                if (item.complete && !item.activated)
                    ticks += 45;
            }
            return ticks;
        }

        void SetSculptureIngredientsActive()
        {
            foreach (var item in currentSculpture.ingredients)
            {
                if (item.complete && !item.activated)
                    item.activated = true;
            }
            
        }

       void WalkToNode(GOAD_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenNodes(this);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {

                agent.NodeDeviate(this);
                return;

            }

            if (agent.nodePath.Count > 0)
            {
                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }

            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.02f)
            {
                if (agent.currentPathIndex < agent.nodePath.Count - 1)
                {
                    agent.currentPathIndex++;
                    agent.currentNode = agent.nodePath[agent.currentPathIndex];
                    agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
                }
                else if (agent.currentPathIndex >= agent.nodePath.Count - 1)
                {
                    agent.lastValidNode = agent.currentNode;
                    if (!atNode)
                        atNode = true;
                    else
                        isDone = true;
                }
            }
            agent.walker.SetLastPosition();
        }

        void GetPathToNode(GOAD_Scheduler_NPC agent, NavigationNode node)
        {

            agent.nodePath.Clear();
            agent.currentPathIndex = 0;
            agent.nodePath = agent.currentNode.FindPath(node);
            agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
        }


        void WalkToSculpture(GOAD_Scheduler_NPC agent)
        {

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);


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
                    atSculpture = true;
                }
            }

            agent.walker.SetLastPosition();
        }
    } 
}
