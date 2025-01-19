using Klaxon.Interactable;
using System.Collections;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_RunMuseum : GOAD_Action
    {

        public InteractableChair workSeat;
        NavigationNode target;
        public GOAD_ScriptableCondition paintingAvailableCondition;
        MuseumManager museum;
        bool hasPainting;

        bool sitting;
        bool gettingUp;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            sitting = true;
            gettingUp = false;
            hasPainting = false;
            // Setting up the seat for when we leave
            
            target = workSeat.findNode;
            agent.currentNode = workSeat.sitNode;
            workSeat.canInteract = false;
            agent.nodePath.Clear();
            agent.currentPathIndex = 0;
            agent.nodePath = agent.currentNode.FindPath(target);

            museum = MuseumManager.instance;

        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);

            
            if (agent.HasBelief("CanWork", false))
            {
                GetUpAndLeave(agent);
                return;
            }

            
            
            if(!hasPainting)
            {
                if(Time.frameCount % 60 == 0)
                    hasPainting = museum.HasPaintingsInQueue();
                return;
            }
            if (sitting)
            {
                if (!gettingUp)
                {
                    agent.animator.SetBool(agent.isSitting_hash, false);
                    StartCoroutine(PlaceNPC(agent, workSeat.sitNode.transform.position, true));
                }
                return;
            }
            agent.currentNode = workSeat.sitNode;
            agent.SetBeliefState(paintingAvailableCondition.Condition, paintingAvailableCondition.State);
            success = true;
            agent.SetActionComplete(true);

        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            workSeat.canInteract = true;
        }


        private void GetUpAndLeave(GOAD_Scheduler_NPC agent)
        {
            if (sitting)
            {
                if (!gettingUp)
                {
                    agent.animator.SetBool(agent.isSitting_hash, false);
                    StartCoroutine(PlaceNPC(agent, workSeat.sitNode.transform.position, true));

                }
                return;
            }

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.nodePath.Count > 0)
            {
                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }
            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.01f)
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
                    success = false;
                    agent.SetActionComplete(true);
                    // Set AtWork belief to false
                }
            }

            agent.walker.SetLastPosition();

        }

        IEnumerator PlaceNPC(GOAD_Scheduler_NPC agent, Vector3 position, bool standingUp)
        {
            float timer = 0;
            float maxTime = 0.45f;
            gettingUp = true;
            while (timer < maxTime)
            {
                Vector3 pos = Vector3.Lerp(agent.transform.position, position, timer / maxTime);
                agent.transform.position = pos;
                timer += Time.deltaTime;
                yield return null;
            }
            sitting = false;
            gettingUp = false;
            yield return null;
        }


    }
}
