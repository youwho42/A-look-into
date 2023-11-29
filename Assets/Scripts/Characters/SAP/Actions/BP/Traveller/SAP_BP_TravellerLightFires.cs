using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;


namespace Klaxon.SAP
{
    public class SAP_BP_TravellerLightFires : SAP_Action
    {
        List<Campfire> fires = new List<Campfire>();
        int lightIndex = 0;
        bool atFire;
        bool hasLicked;
        float timer;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            fires = FindFires(agent);
            agent.arms.SetActive(false);
            agent.animator.SetBool(agent.walking_hash, true);
            agent.SetBeliefState("IsHome", false);
        }

        public override void PerformAction(SAP_Scheduler_BP agent)
        {
            if (atFire)
            {

                if (!hasLicked)
                {
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDirection = Vector2.zero;
                    agent.animator.SetTrigger(agent.lick_hash);
                    hasLicked = true;
                    fires[lightIndex].Interact(gameObject);
                }


                if (timer < 1f)
                    timer += Time.deltaTime;
                else
                {
                    if (lightIndex < fires.Count)
                    {
                        atFire = false;
                        hasLicked = false;
                        timer = 0;
                        lightIndex++;
                    }
                    if (lightIndex == fires.Count)
                        agent.currentGoalComplete = true;
                        
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

            if (lightIndex < fires.Count)
            {
                agent.walker.hasDeviatePosition = false;

                agent.walker.SetWorldDestination(GetFiresPosition(agent, fires[lightIndex]));
                agent.walker.SetDirection();
                if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                    atFire = true;
                 
                agent.walker.SetLastPosition();
            }
            
        }

        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            agent.SetBeliefState("FiresLit", fires[0].isLit);
            fires.Clear();
            lightIndex = 0;
            atFire = false;
            hasLicked = false;
            timer = 0;
        }

        List<Campfire> FindFires(SAP_Scheduler_BP agent)
        {
            List<Campfire> all = new List<Campfire>();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1, agent.interactableLayer);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].transform.position.z != transform.position.z)
                        continue;

                    if (colliders[i].gameObject.TryGetComponent(out Campfire fire))
                        all.Add(fire);
                }
            }
            return all;
        }


        Vector3 GetFiresPosition(SAP_Scheduler_BP agent, Campfire fire)
        {
            Vector3 pos = fire.transform.position;
            Vector2 dir = transform.position - pos;
            dir = dir.normalized;
            dir *= 0.12f;
            var colliders = fire.GetComponentsInChildren<Collider2D>();
            foreach (var coll in colliders)
            {
                if (gameObject.layer == agent.obstacleLayer)
                {
                    pos = coll.ClosestPoint(transform.position);
                    break;
                }
            }

            return pos + (Vector3)dir;
        }
    }

}
