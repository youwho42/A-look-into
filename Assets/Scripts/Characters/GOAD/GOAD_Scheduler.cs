using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Scheduler : MonoBehaviour
    {
        [Serializable]
        public struct PossibleGoal
        {
            public string Name;
            public GOAD_MainGoal DesiredGoal;
            public int Priority;
        }

        protected List<GOAD_Action> availableActions = new List<GOAD_Action>();
        public List<PossibleGoal> possibleGoals = new List<PossibleGoal>();
        public Dictionary<string, bool> beliefs = new Dictionary<string, bool>();

        protected Queue<GOAD_Action> actionQueue = new Queue<GOAD_Action>();

        protected int currentGoalIndex = -1;
        protected GOAD_Action currentAction;
        protected bool currentActionComplete;
        public string currentActionName;

        GOAD_WorldBeliefStates worldBeliefs;

        protected Transform _transform;
        public void SetActionComplete(bool state)
        {
            currentActionComplete = state;
        }
        public virtual void Start()
        {
            _transform = transform;
            availableActions = GetComponents<GOAD_Action>().ToList();
            worldBeliefs = GOAD_WorldBeliefStates.instance;
        }
        

        protected void GetCurrentGoal()
        {
            currentGoalIndex = -1;
            int currentPriority = -1;
            
            // Loop through all possible goals
            for (int i = 0; i < possibleGoals.Count; i++)
            {
                if (AreConditionsMet(possibleGoals[i].DesiredGoal.PreConditions))
                {
                    if (possibleGoals[i].Priority >= currentPriority)
                    {
                        currentPriority = possibleGoals[i].Priority;
                        currentGoalIndex = i;
                    }
                }
            }
            
            if (currentGoalIndex > -1)
            {
                List<GOAD_Action> allActions = GetActionsNeededToCompleteGoal(GetPossibleAction(possibleGoals[currentGoalIndex].DesiredGoal.ResultCondition));
                allActions.Reverse();
                foreach (var action in allActions)
                {
                    actionQueue.Enqueue(action);
                }
            }
            
        }

        
        protected void ResetGoal()
        {
            actionQueue.Clear();
            currentGoalIndex = -1;
            currentAction = null;
        }

        List<GOAD_Action> GetActionsNeededToCompleteGoal(GOAD_Action action, List<GOAD_Action> actionList = null)
        {
            if (actionList == null)
            {
                actionList = new List<GOAD_Action>();
            }
            if (action == null)
                return actionList;
            
            actionList.Add(action);
            // Base case: if no possible action or if the goal's condition is already met
            if (IsConditionMet(action.Goal.PreCondition))
            {
                return actionList;
            }
            else
            {
                var nextPossibleAction = GetPossibleAction(action.Goal.PreCondition);
                if(nextPossibleAction != null) 
                    GetActionsNeededToCompleteGoal(nextPossibleAction, actionList);
            }

            return actionList;
        }

        GOAD_Action GetPossibleAction(GOAD_ScriptableCondition desiredCondition)
        {
            foreach (var action in availableActions)
            {
                if (CompareConditions(action.Goal.ResultCondition, desiredCondition))
                    return action;
            }
            return null;
        }

        public bool IsConditionMet(GOAD_ScriptableCondition condition)
        {
            Dictionary<string, bool> temp = new Dictionary<string, bool>(beliefs);

            // Combine the two dictionaries without modifying the original dictionaries
            foreach (var kvp in worldBeliefs.worldStates)
            {
                    temp[kvp.Key] = kvp.Value;
            }

            // Check if the condition's key exists in the combined dictionary and compare the state
            if (temp.TryGetValue(condition.Condition, out bool state))
            {
                return condition.State == state;
            }

            return false;
        }

        public bool AreConditionsMet(List<GOAD_ScriptableCondition> conditions)
        {
            Dictionary<string, bool> temp = new Dictionary<string, bool>(beliefs);

            // Combine the two dictionaries without modifying the original dictionaries
            foreach (var kvp in worldBeliefs.worldStates)
            {
                    temp[kvp.Key] = kvp.Value;
            }

            // Check if each condition's key exists in the combined dictionary and compare the state
            bool allMet = true;
            foreach (var condition in conditions)
            {
                if (temp.TryGetValue(condition.Condition, out bool state))
                    if (condition.State == state)
                        continue;
                    else
                    {
                        allMet = false;
                        break;
                    }

                allMet = false;
                break;
            }
            

            return allMet;
        }

        bool CompareConditions(GOAD_ScriptableCondition conditionA, GOAD_ScriptableCondition conditionB)
        {
            return conditionA.Condition == conditionB.Condition && conditionA.State == conditionB.State;
        }

        public void SetBeliefState(string condition, bool state)
        {
            if (!beliefs.ContainsKey(condition))
                beliefs.Add(condition, state);
            else
                beliefs[condition] = state;
        }

        public void SetBeliefState(GOAD_ScriptableCondition condition)
        {
            if (!beliefs.ContainsKey(condition.Condition))
                beliefs.Add(condition.Condition, condition.State);
            else
                beliefs[condition.Condition] = condition.State;
        }

        public bool HasBelief(string condition, bool state)
        {
            if (beliefs.ContainsKey(condition))
            {
                return beliefs[condition] == state;
            }
            return false;
        }
    }
}

