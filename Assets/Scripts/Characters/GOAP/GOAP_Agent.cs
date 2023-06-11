using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using QuantumTek.QuantumInventory;
using System;

namespace Klaxon.GOAP
{
	
	public class GOAP_Goal
	{
		public Dictionary<string, int> goals;
		public bool remove;

        
        public GOAP_Goal(string s, int i, bool r)
		{
			goals = new Dictionary<string, int>();
			goals.Add(s, i);
			remove = r;
		}
	}
	public class GOAP_Agent : MonoBehaviour
	{

		public List<GOAP_Action> actions = new List<GOAP_Action>();
		public Dictionary<GOAP_Goal, int> goals = new Dictionary<GOAP_Goal, int>();
		public GOAP_WorldStates beliefs = new GOAP_WorldStates();
		public GOAP_World world;

		GOAP_Planner planner;
		GOAP_Planner pausedPlanner;
		Queue<GOAP_Action> actionQueue;
		Queue<GOAP_Action> pausedActionQueue;
		public GOAP_Action currentAction;
		public GOAP_Action pausedAction;
		GOAP_Goal currentGoal;
		bool invoked = false;
		public float wanderDistance;

		[HideInInspector]
		public QI_Inventory agentInventory;
		[HideInInspector]
        public NavigationNodeType currentNavigationNodeType;
		public NavigationPathType pathType;

        public Animator animator;
        public bool destinationReached = false;
        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int velocityX_hash = Animator.StringToHash("VelocityX");
        public readonly int velocityY_hash = Animator.StringToHash("VelocityY");
		public bool offScreen;
        protected virtual void Start()
        {
			agentInventory = GetComponent<QI_Inventory>();
			GOAP_Action[] acts = GetComponents<GOAP_Action>();
			foreach (var a in acts)
			{
				actions.Add(a);
			}
        }

		void LateUpdate()
		{
			
			if (destinationReached)
			{
				return;
			}



			if (currentAction != null && currentAction.running)
			{
				currentAction.Perform(this);
				

				// when we reach our goal, do the action of what we want to happen (gather seed, drop seed off, etc...)
				if (destinationReached)
				{
					if (!invoked)
					{
						currentAction.PrePostPerform(this);
						Invoke("CompleteAction", currentAction.GetFinalDuration());
						invoked = true;
					}
				}

				return;
			} 
			


            if(planner == null || actionQueue == null)
			{
				planner = new GOAP_Planner();

				var sortedGoals = from entry in goals orderby entry.Value descending select entry;
				foreach (var goal in sortedGoals)
				{
                    actionQueue = planner.Plan(actions, goal.Key.goals, beliefs, world);
					if(actionQueue != null)
					{
						currentGoal = goal.Key;
						break;
					}
				}
			}

			if(actionQueue != null && actionQueue.Count == 0) 
			{
                
                if (currentGoal.remove)
					goals.Remove(currentGoal);
				planner = null;
			}

			if(actionQueue != null && actionQueue.Count > 0)
			{
				currentAction = actionQueue.Dequeue();
				if (currentAction.PrePerform(this))
				{
					
                    currentAction.running = true;
                }
				else
				{
					actionQueue = null;
				}
			}
        }

		public void CancelAction()
		{
			if(currentAction != null) 
				currentAction.running = false;
            currentAction = null;
			actionQueue = null;
        }

		void CompleteAction()
		{
			destinationReached = false;
            
            currentAction.running = false;
            currentAction.PostPerform(this);
            
            invoked = false;
		}

        public void OnTriggerEnter2D(Collider2D collision)
        {
           
            if (collision.CompareTag("House"))
			{
				if(collision.OverlapPoint(transform.position))
                    currentNavigationNodeType = NavigationNodeType.Inside;
            }
				

            
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.transform.position.z != transform.position.z)
                    return;
                pausedPlanner = planner;
				pausedAction = currentAction;
				if(actionQueue!=null)
					pausedActionQueue = new Queue<GOAP_Action>(actionQueue);
				CancelAction();
                beliefs.SetState("TalkToPlayer", 0);
            }
        }

		

            public void OnTriggerExit2D(Collider2D collision)
        {
            
            if (collision.CompareTag("House"))
                currentNavigationNodeType = NavigationNodeType.Outside;

            
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.transform.position.z != transform.position.z)
                    return;
                planner = pausedPlanner;
				currentAction = pausedAction;
				if (currentAction != null)
					currentAction.running = true;
				if (pausedActionQueue != null)
				{
                    actionQueue = new Queue<GOAP_Action>(pausedActionQueue);
					pausedActionQueue = null;
                }
					
                beliefs.RemoveState("TalkToPlayer");
            }
        }

    }


}