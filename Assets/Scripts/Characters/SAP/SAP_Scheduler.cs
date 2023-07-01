using Klaxon.GravitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Scheduler : MonoBehaviour
    {

        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int isSitting_hash = Animator.StringToHash("IsSitting");
        public readonly int isSleeping_hash = Animator.StringToHash("IsSleeping");
        public readonly int velocityX_hash = Animator.StringToHash("VelocityX");
        public readonly int velocityY_hash = Animator.StringToHash("VelocityY");
        public Animator animator;
        
        public List<SAP_Goal> goals = new List<SAP_Goal>();

        Dictionary<string, bool> beliefs = new Dictionary<string, bool>();

        int currentGoal = -1;

        [HideInInspector]
        public NavigationNodeType currentNavigationNodeType;
        [HideInInspector]
        public NavigationPathType pathType;
        [HideInInspector]
        public bool currentGoalComplete;
        public string currentGoalName;
        [HideInInspector]
        public bool offScreen;
        float currentGoalTimer;
        [HideInInspector]
        public NavigationNode lastValidNode;
        bool isTalking;

        [HideInInspector]
        public GravityItemWalker walker;
        private void Start()
        {
            walker = GetComponent<GravityItemWalker>();
        }

        private void Update()
        {
            if (isTalking)
            {
                
                return;
            }
            if (currentGoal == -1)
            {
                SetNewGoal();
            }
            else
            { 
                goals[currentGoal].Action.PerformAction(this);

                if(goals[currentGoal].TimeLimit > 0)
                {
                    currentGoalTimer += Time.deltaTime;
                    if(currentGoalTimer >= goals[currentGoal].TimeLimit) 
                    {
                        SetBeliefState(goals[currentGoal].TimeLimitCondition.Condition, goals[currentGoal].TimeLimitCondition.State);
                        goals[currentGoal].Action.EndPerformAction(this);
                        currentGoal = -1;
                        currentGoalComplete = false;
                        currentGoalTimer = 0;
                    }
                }
                    


            }

            if (currentGoalComplete && currentGoal >= 0)
            {
                goals[currentGoal].Action.EndPerformAction(this);
                currentGoal = -1;

                currentGoalComplete = false;
            }
        }

        void SetNewGoal()
        {
            
            int bestOption = -1;
            int bestIndex = -1;
            for (int i = 0; i < goals.Count; i++)
            {
                goals[i].IsRunning = false;
                if (CanCompleteGoal(goals[i]))
                {
                    if (goals[i].Priority > bestOption)
                    {
                        bestOption = goals[i].Priority;
                        bestIndex = i;
                    }

                }
            }
            
                

            if(bestIndex > -1)
            {

                if (currentGoal != bestIndex)
                {
                    if (currentGoal > -1)
                        goals[currentGoal].Action.EndPerformAction(this);

                    goals[bestIndex].Action.StartPerformAction(this);
                }

                currentGoal = bestIndex;
                goals[currentGoal].IsRunning = true;

                if (currentGoalName != goals[currentGoal].GoalName)
                    currentGoalTimer = 0;
                
                currentGoalName = goals[currentGoal].GoalName;
            }
            else
            {
                currentGoalName = "";
                currentGoalTimer = 0;
            }
            
        }

        bool CanCompleteGoal(SAP_Goal goal)
        {
            Dictionary<string, bool> temp = new Dictionary<string, bool>(beliefs);

            // Combine the two dictionaries without modifying the original dictionaries
            foreach (var kvp in SAP_WorldBeliefStates.instance.worldStates)
            {
                temp[kvp.Key] = kvp.Value;
            }
            int conditionsMet = 0;
            //bool canComplete = true;
            foreach (var c in goal.Conditions)
            {
                bool state;
                if (temp.TryGetValue(c.Condition, out state))
                {
                    if (c.State == state)
                    {
                        conditionsMet++;
                    }
                }
            }
            return conditionsMet == goal.Conditions.Count;
        }





        public void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.CompareTag("House"))
            {
                if (collision.OverlapPoint(transform.position))
                    currentNavigationNodeType = NavigationNodeType.Inside;
            }



            if (collision.gameObject.CompareTag("Player"))
            {
                isTalking = true;
                animator.SetFloat(velocityX_hash, 0);
                walker.currentDir = Vector2.zero;
                if (collision.transform.position.x < transform.position.x && walker.facingRight ||
                    collision.transform.position.x > transform.position.x && !walker.facingRight)
                    walker.Flip();
                
            }
        }



        public void OnTriggerExit2D(Collider2D collision)
        {

            if (collision.CompareTag("House"))
                currentNavigationNodeType = NavigationNodeType.Outside;


            if (collision.gameObject.CompareTag("Player"))
            {
                isTalking = false;
            }
        }

        public void SetBeliefState(string condition, bool state)
        {
            if (!beliefs.ContainsKey(condition))
            {
                beliefs.Add(condition, state);
            }
            else
            {
                beliefs[condition] = state;
            }
        }


    }
}
