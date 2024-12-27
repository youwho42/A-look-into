using Klaxon.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Klaxon.GOAD
{
    public class GOAD_Action_SetFires : GOAD_Action
    {
        
        bool atFire;
        bool hasLicked;
        float timer;
        Vector3 currentFireDirection;

        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.arms.SetActive(false);
            agent.animator.SetBool(agent.sleeping_hash, false);
            agent.animator.SetBool(agent.walking_hash, true);
            currentFireDirection = GetFiresPosition(agent);
            ContextSpeechBubbleManager.instance.SetContextBubble(2, agent.speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "FIRE"), false);

        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);

            if (atFire)
            {

                if (!hasLicked)
                {
                    agent.currentFire.BPSetFire();
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDirection = Vector2.zero;
                    agent.animator.SetTrigger(agent.lick_hash);
                    hasLicked = true;
                }

                if (timer < 1f)
                    timer += Time.deltaTime;
                else
                {
                    success = true;
                    agent.SetActionComplete(true);
                }
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }

            agent.walker.hasDeviatePosition = false;
            currentFireDirection = GetFiresPosition(agent);
            agent.walker.SetWorldDestination(currentFireDirection);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                atFire = true;

            agent.walker.SetLastPosition();
            
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            atFire = false;
            hasLicked = false;
            timer = 0;
            agent.currentFire = null;
            agent.walker.isStuck = false;
            agent.isDeviating = false;
        }



        Vector3 GetFiresPosition(GOAD_Scheduler_BP agent)
        {
            Vector3 pos = agent.currentFire.transform.position;
            Vector2 dir = transform.position - pos;
            dir = dir.normalized;
            dir *= 0.2f;
            var colliders = agent.currentFire.GetComponentsInChildren<Collider2D>();
            foreach (var coll in colliders)
            {
                if (gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    pos = coll.ClosestPoint(transform.position);
                    break;
                }
            }

            return pos + (Vector3)dir;
        }
    }
}
