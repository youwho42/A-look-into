using Klaxon.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_Sleep : GOAD_Action
    {
        GOAD_WorldBeliefStates worldStates;

        public InteractableDialogue interactableDialogue;
        public NPC_UndertakingAvailable undertakingAvailable;

        bool sleeping;

        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);

            worldStates = GOAD_WorldBeliefStates.instance;
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);


        }
        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);

            if (worldStates.worldStates.TryGetValue("Day", out bool isDay))
            {
                if (isDay)
                {
                    success = true;
                    agent.SetActionComplete(true);
                    return;
                }
            }
                

            if (sleeping)
                return;

            if (!sleeping)
            {
                sleeping = true;
                interactableDialogue.canInteract = false;
                if (undertakingAvailable != null)
                {
                    undertakingAvailable.isInactive = true;
                    undertakingAvailable.SetUndertakingIcon(); 
                }
                agent.animator.SetBool(agent.isSleeping_hash, true);
                agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                agent.animator.SetFloat(agent.velocityX_hash, 0);
                agent.walker.currentDirection = Vector2.zero;
                Vector3 displacement = new Vector3(agent.walker.transform.position.x, agent.walker.transform.position.y, agent.walker.transform.position.z + 0.33f);
                agent.walker.transform.position = displacement;
                return;
            }



        }
        
        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            sleeping = false;
            interactableDialogue.canInteract = true;
            if (undertakingAvailable != null)
            {
                undertakingAvailable.isInactive = false;
                undertakingAvailable.SetUndertakingIcon(); 
            }
        }
    }

}