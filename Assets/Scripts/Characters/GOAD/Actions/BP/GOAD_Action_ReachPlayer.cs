using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_ReachPlayer : GOAD_Action
	{
        Vector2 offset;

        Vector3 lastDestination;
        float timer;
        
        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, true);
            timer = 0;
            offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
            
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);

            if (!agent.CheckNearPlayer(3))
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            // what if BP is a locationer and the player hasn't interacted with BP yet also reached the final destination?
            if(agent.locationerLocation != null)
            {
                var dist = (transform.position-agent.locationerLocation.position).sqrMagnitude;

                if(dist <= 1f)
                {
                    agent.SetBeliefState(agent.questComplteCondition.Condition, agent.questComplteCondition.State);
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDirection = Vector2.zero;
                    ContextSpeechBubbleManager.instance.SetContextBubble(3, agent.speechBubbleTransform, "Well, I guess you showed me where I wanted to show you!!" /*LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorThisWay")*/, false);
                    timer += Time.deltaTime;
                    if (timer > 3.5f)
                    {
                        success = true;
                        agent.SetActionComplete(true);
                    }
                    return;
                }

            }

            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (lastDestination == agent.walker.currentDestination)
                {
                    offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                }
                lastDestination = agent.walker.currentDestination;

                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }

            agent.walker.hasDeviatePosition = false;
            


            agent.walker.currentDestination = PlayerInformation.instance.player.position + (Vector3)offset;

            agent.walker.SetWorldDestination(agent.walker.currentDestination);
            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {
                success = true;
                agent.SetActionComplete(true);
            }

            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.walker.isStuck = false;
            agent.isDeviating = false;
        }
    }

}