using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Klaxon.GOAD
{
	public class GOAD_Action_ChillNearPlayer : GOAD_Action
	{

        float timeIdle;
        bool sleeping;
        bool hasSpoken;
        public List<LocalizedString> localizedStrings = new List<LocalizedString>();
        bool lastInteractionState;
        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.interactor.canInteract = true;
            hasSpoken = false;
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.walker.ResetLastPosition();
            lastInteractionState = agent.hasInteracted;
            var player = PlayerInformation.instance.player;
            if (player.position.x < transform.position.x && agent.walker.facingRight ||
                player.position.x > transform.position.x && !agent.walker.facingRight)
                agent.walker.Flip();
        }
        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if(lastInteractionState == false && agent.hasInteracted)
            {
                lastInteractionState = agent.hasInteracted;
                success = true;
                agent.SetActionComplete(true);
                return;
                
            }

            if (!agent.CheckNearPlayer(sleeping ? 0.85f : 0.5f))
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            if (sleeping)
            {
                agent.interactor.canInteract = false;
                agent.animator.SetBool(agent.sleeping_hash, true);
                return;
            }

            var dir = PlayerInformation.instance.player.position - transform.position;
            agent.walker.SetFacingDirection(dir);
            agent.arms.SetActive(!agent.hasInteracted);
            agent.interactor.canInteract = !agent.hasInteracted;
            if (agent.interactor.canInteract && !hasSpoken)
            {
                hasSpoken = true;
                if (Random.value < .18f)
                {
                    int r = Random.Range(0, localizedStrings.Count);
                    ContextSpeechBubbleManager.instance.SetContextBubble(1.5f, agent.speechBubbleTransform, localizedStrings[r].GetLocalizedString(), false);
                }
                    
            }

            timeIdle += Time.deltaTime;

            
            if (timeIdle >= 10.0f)
                sleeping = true;
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            timeIdle = 0;
            sleeping = false;
            agent.animator.SetBool(agent.sleeping_hash, false);
            
        }
    } 
}
