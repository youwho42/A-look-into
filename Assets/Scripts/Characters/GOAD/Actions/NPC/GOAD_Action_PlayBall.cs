using Klaxon.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_PlayBall : GOAD_Action
    {
        public Transform goalPost;
        public Transform ball;
        bool pursueBall;
        bool kickBall;
        bool atSide;
        float sideRange;
        float oppositeRange;

        bool running = true;
        float walkTime = 0;
        public bool inField;
        bool isAtPosition;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            
            agent.interactable.canInteract = false;
            agent.animator.SetFloat(agent.velocityX_hash, 1);
            agent.animator.SetBool(agent.isRunning_hash, true);
            agent.walker.isRunning = true;
            sideRange = Random.Range(0.25f, 0.35f);
            oppositeRange = Random.Range(0.1f, 0.25f);
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.walker.isStuck || agent.isDeviating)
            {
                agent.Deviate();
                return;
            }

            pursueBall = inField ? GetGoalDistance() <= 1.5 * 1.5 : GetGoalDistance() > 1.5 * 1.5;


            if (!running)
                walkTime += Time.deltaTime;
            else
            {
                agent.animator.SetBool(agent.isRunning_hash, true);
                agent.walker.isRunning = true;
            }
                

            if (walkTime > 4)
            {
                agent.animator.SetBool(agent.isRunning_hash, true);
                agent.walker.isRunning = true;
                walkTime = 0;
                running = true;
            }
            
            if(pursueBall)
            {
                if (!atSide && !kickBall)
                {

                    agent.walker.currentDestination = GetSidePosition(sideRange);
                    agent.walker.SetDirection();

                    if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                    {
                        sideRange = Random.Range(0.25f, 0.35f);
                        atSide = true;
                    }
                    agent.walker.SetLastPosition();
                    return;
                }
                if (atSide && !kickBall)
                {

                    agent.walker.currentDestination = GetOppositePosition(oppositeRange);
                    agent.walker.SetDirection();

                    if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                    {
                        oppositeRange = Random.Range(0.1f, 0.25f);
                        agent.walker.currentDestination = ball.position;
                        kickBall = true;
                    }
                    agent.walker.SetLastPosition();
                    return;
                }
                if (atSide && kickBall)
                {

                    agent.walker.currentDestination = ball.position;
                    agent.walker.SetDirection();
                    agent.walker.addUpsies = GetGoalDistance() >= 1;
                    if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                    {
                        kickBall = false;
                        atSide = false;
                        if (Random.Range(0.0f, 1.0f) < 0.3f)
                        {
                            running = false;
                            agent.animator.SetBool(agent.isRunning_hash, false);
                            agent.walker.isRunning = false;
                        }
                    }
                    agent.walker.SetLastPosition();
                    return;
                }

            }
            else
            {
                
                if (!isAtPosition)
                {
                    agent.walker.currentDestination = GetFieldPosition(sideRange);
                    agent.walker.SetDirection();

                    agent.walker.SetLastPosition();
                    if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                    {
                        isAtPosition = true;
                    }
                    return;
                }
                else
                {
                    FaceBall(agent);
                    var pos = GetFieldPosition(sideRange);
                    agent.animator.SetFloat(agent.velocityX_hash, 0);
                    agent.animator.SetBool(agent.isRunning_hash, false);
                    agent.walker.currentDirection = Vector3.zero;
                    if (Vector3.Distance(transform.position, pos) > 0.5f)
                    {
                        isAtPosition = false;
                    }
                }
                
            }



        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.interactable.canInteract = true;
            agent.animator.SetBool(agent.isRunning_hash, false);
            agent.walker.isRunning = false;
            agent.walker.isStuck = false;
            agent.isDeviating = false;
        }

        void FaceBall(GOAD_Scheduler_NPC agent)
        {
            if (ball.position.x > transform.position.x && !agent.walker.facingRight)
                agent.walker.Flip();
            if (ball.position.x < transform.position.x && agent.walker.facingRight)
                agent.walker.Flip();
        }

        Vector3 GetFieldPosition(float range)
        {

            
            Vector2 direction = (ball.position - goalPost.position).normalized;
            float dist = inField ? Random.Range(.1f, 1f) : Random.Range(1f, 1.6f);
            direction = direction * dist;
            return goalPost.position + (Vector3)direction;
        }
        Vector3 GetSidePosition(float range)
        {

            Vector2 playerToGoal = (transform.position - goalPost.position).normalized;
            Vector2 direction = (ball.position - goalPost.position).normalized;
            if(Vector2.Dot(direction, playerToGoal)<=0)
                direction = Vector2.Perpendicular(direction);
            direction = direction * range;
            return ball.position + (Vector3)direction;
        }
        Vector3 GetOppositePosition(float range)
        {
            Vector2 direction = (ball.position - goalPost.position).normalized;
            direction = direction * range;
            return ball.position + (Vector3)direction;
        }

        float GetGoalDistance()
        {
            return (goalPost.position - ball.position).sqrMagnitude;
        }
    }
}
