using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_ClimbTree : SAP_Action
	{

        

        float timeToStayAtDestination;

        TreeRustling currentClimable;
        bool climbing;

        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            timeToStayAtDestination = agent.SetRandomRange(new Vector2(5.0f, 15.0f));

            

            agent.walker.enabled = true;

            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }


            agent.walker.currentDirection = Vector2.zero;

            agent.animator.SetBool(agent.walking_hash, false);

            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false;
            }

            
            CheckCurrentTree();
            if (currentClimable == null)
            {
                agent.currentGoalComplete = true;
                return;
            }
            agent.walker.SetDestinationZ(agent.currentDisplacementSpot);

            climbing = true;
            agent.walker.isClimbing = true;

            var dir = currentClimable.transform.position - transform.position;
            var d = Mathf.Sign(dir.x);
            agent.walker.characterRenderer.flipX = d > 0 && !agent.walker.facingRight;
            agent.walker.isWeightless = true;
            agent.animator.SetBool(agent.climbing_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            

        }

        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {
            
            

            if(!climbing && timeToStayAtDestination > 0)
            {
                timeToStayAtDestination -= Time.deltaTime;
                if (timeToStayAtDestination < 0)
                {
                    climbing = true;
                    agent.walker.isClimbing = true;
                    agent.animator.SetBool(agent.climbIdle_hash, false);
                    agent.walker.ResetDestinationZ();
                    SetDirectionZ(agent);
                    
                }
            }
            

            if (climbing)
            {
                agent.animator.SetBool(agent.climbing_hash, true);
                agent.animator.SetBool(agent.walking_hash, false);
                
                if (Vector2.Distance(agent.walker.itemObject.localPosition, agent.walker.currentDestinationZ) <= 0.02f)
                {
                    
                    climbing = false;
                    if (agent.walker.currentDestinationZ.z == 0)
                    {
                        agent.walker.itemObject.localPosition = Vector3.zero;
                        agent.walker.isClimbing = false;
                        agent.walker.isWeightless = false;
                        agent.walker.characterRenderer.flipY = false;
                        agent.walker.characterRenderer.flipX = false;
                        agent.currentGoalComplete = true;
                    }
                    else
                    {
                        
                        agent.animator.SetBool(agent.climbIdle_hash, true);
                        agent.walker.isClimbing = false;
                        currentClimable.Affect();
                        
                    }

                }
            }

        }


        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            agent.animator.SetBool(agent.climbing_hash, false);
            agent.SetBeliefState("CanClimb", false);
            agent.SetBeliefState("AtClimbable", false);
            currentClimable = null;
            agent.currentDisplacementSpot.isInUse = false;

        }

        void SetDirectionZ(SAP_Scheduler_ANIMAL agent)
        {
            var dirZ = Mathf.Sign(agent.walker.currentDirectionZ.z);
            agent.walker.characterRenderer.flipY = dirZ < 0;
        }

        void CheckCurrentTree()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, .1f);
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent(out TreeRustling tree))
                    {
                        currentClimable = tree;
                    }

                }
            }
        }

    }

}