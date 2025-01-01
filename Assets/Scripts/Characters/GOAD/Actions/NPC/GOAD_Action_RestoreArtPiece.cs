using Klaxon.Interactable;
using System.Collections;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_RestoreArtPiece : GOAD_Action
    {
        public InteractableChair workSeat;
        RestorePainting currentPainting;
        bool atPainting;
        bool isRestoring;

        bool returnToSeat;
        bool destinationReached;
        bool sitting;

        CycleTicks currentWaitTime;
        RealTimeDayNightCycle dayNightCycle;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            dayNightCycle = RealTimeDayNightCycle.instance;
            atPainting = false;
            isRestoring = false;
            returnToSeat = false;

            sitting = false;
            destinationReached = false;

            currentPainting = MuseumManager.instance.GetNextPainting();
            if (currentPainting == null)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }
            GetPathToNode(agent, currentPainting.paintingNode);
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);

            if (sitting)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }



            if (returnToSeat)
            {
                WalkToSeat(agent);
                return;
            }

            if (!atPainting)
            {
                WalkToPainting(agent);
                return;
            }

            if (!isRestoring)
            {
                currentWaitTime = dayNightCycle.GetCycleTime(GetPaintingRestoreTicks(agent));
                currentPainting.interactablePainting.canInteract = false;
                agent.animator.SetBool(agent.isCrafting_hash, true);
                isRestoring = true;
            }

            if (dayNightCycle.currentTimeRaw >= currentWaitTime.tick && dayNightCycle.currentDayRaw == currentWaitTime.day)
            {
                isRestoring = false;
                agent.animator.SetBool(agent.isCrafting_hash, false);
                currentPainting.interactablePainting.canInteract = true;
                currentPainting.GetIsFinished();
                SetPaintingLayersActive();
                currentPainting = MuseumManager.instance.GetNextPainting();
                if (currentPainting == null)
                {
                    GetPathToNode(agent, workSeat.sitNode);
                    returnToSeat = true;
                    return;
                }
                GetPathToNode(agent, currentPainting.paintingNode);
                atPainting = false;
            }

        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
        }
        void SetPaintingLayersActive()
        {
            foreach (var item in currentPainting.ingredients)
            {
                if (item.complete && !item.activated)
                    item.activated = true;
            }
            MuseumManager.instance.RemovePaintingFromQueue();
        }

        int GetPaintingRestoreTicks(GOAD_Scheduler_NPC agent)
        {
            int ticks = 0;
            foreach (var item in currentPainting.ingredients)
            {
                if (item.complete && !item.activated)
                    ticks += 10;
            }
            return ticks;
        }

        void WalkToPainting(GOAD_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);
            //// this is where we need to make the npc GO TO the destination.
            //// use currentAction.walker here

            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenNodes(this);
                return;
            }


            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.NodeDeviate(this);
                    return;
                }
            }



            if (agent.nodePath.Count > 0)
            {
                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }
            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.03f)
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
                    atPainting = true;
                    agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                    agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                    agent.animator.SetFloat(agent.velocityX_hash, 0);
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



        void WalkToSeat(GOAD_Scheduler_NPC agent)
        {
            if (sitting)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }


            if (destinationReached && !sitting)
            {
                agent.offScreenPosMoved = false;
                sitting = true;
                agent.animator.SetBool(agent.isSitting_hash, true);
                agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                agent.animator.SetFloat(agent.velocityX_hash, 0);
                agent.walker.currentDirection = Vector2.zero;
                Vector3 displacement = new Vector3(agent.walker.transform.position.x, agent.walker.transform.position.y, agent.walker.transform.position.z + 0.33f);
                agent.walker.transform.position = displacement;

                agent.lastValidNode = agent.currentNode;
                StartCoroutine(PlaceNPC(agent, workSeat.transform.position, false));
                if (agent.walker.facingRight && !workSeat.facingRight || !agent.walker.facingRight && workSeat.facingRight)
                    agent.walker.Flip();

                return;
            }

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);
            //// this is where we need to make the npc GO TO the destination.
            //// use currentAction.walker here




            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenNodes(this);
                return;
            }


            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.NodeDeviate(this);
                    return;
                }
            }



            if (agent.nodePath.Count > 0)
            {
                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }
            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.03f)
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
                    ReachSeat(agent);

                }
            }

            agent.walker.SetLastPosition();
        }

        public void ReachSeat(GOAD_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            destinationReached = true;
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
        }



        IEnumerator PlaceNPC(GOAD_Scheduler_NPC agent, Vector3 position, bool standingUp)
        {
            float timer = 0;
            float maxTime = 0.45f;

            while (timer < maxTime)
            {
                Vector3 pos = Vector3.Lerp(agent.transform.position, position, timer / maxTime);
                agent.transform.position = pos;
                timer += Time.deltaTime;
                yield return null;
            }


            yield return null;
        }

        public override void OffscreenNodeHandleComplete(GOAD_Scheduler_NPC agent)
        {
            if (returnToSeat)
                ReachSeat(agent);
            else
            {
                agent.lastValidNode = agent.currentNode;
                atPainting = true;
                agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                agent.animator.SetFloat(agent.velocityX_hash, 0);
            }
        }
    }
}
