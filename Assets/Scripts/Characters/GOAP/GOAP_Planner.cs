using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Klaxon.GOAP
{
    public class GOAP_Node
    {
        public GOAP_Node parent;
        public float cost;
        public Dictionary<string, int> state;
        public GOAP_Action action;

        public GOAP_Node(GOAP_Node _parent, float _cost, Dictionary<string, int> _states, GOAP_Action _action)
        {
            parent = _parent;
            cost = _cost;
            state = new Dictionary<string, int>(_states);
            action = _action;
        }
        public GOAP_Node(GOAP_Node _parent, float _cost, Dictionary<string, int> _states, Dictionary<string, int> _beliefs, GOAP_Action _action)
        {
            parent = _parent;
            cost = _cost;
            state = new Dictionary<string, int>(_states);
            foreach (var b in _beliefs)
            {
                if(!state.ContainsKey(b.Key))
                    state.Add(b.Key, b.Value);
            }
            action = _action;
        }
    }

    public class GOAP_Planner
    {
        public Queue<GOAP_Action> Plan(List<GOAP_Action> actions, Dictionary<string, int> goal, GOAP_WorldStates beliefStates, GOAP_World world)
        {
            List<GOAP_Action> usableActions = new List<GOAP_Action>();
            foreach (var action in actions)
            {
                if (action.IsAchievable())
                    usableActions.Add(action);
            }

            List<GOAP_Node> leaves = new List<GOAP_Node>();
            GOAP_Node start = new GOAP_Node(null, 0, world.GetWorld().GetStates(), beliefStates.GetStates(), null);

            bool success = BuildGraph(start, leaves, usableActions, goal);

            if (!success)
            {
                //Debug.Log("No Plan Found");
                return null;
            }

            GOAP_Node cheapest = null;
            foreach (var node in leaves)
            {
                if (cheapest == null)
                    cheapest = node;
                else
                {
                    if (node.cost < cheapest.cost)
                        cheapest = node;
                }
            }

            List<GOAP_Action> result = new List<GOAP_Action>();
            GOAP_Node n = cheapest;
            while (n != null)
            {
                if(n.action != null)
                {
                    result.Insert(0, n.action);
                }
                n = n.parent;
            }

            Queue<GOAP_Action> q = new Queue<GOAP_Action>();
            foreach (var a in result)
            {
                q.Enqueue(a);
            }
            
            return q;
        }

        private bool BuildGraph(GOAP_Node parent, List<GOAP_Node> leaves, List<GOAP_Action> usableActions, Dictionary<string, int> goal)
        {
            bool foundPath = false;
            foreach (var action in usableActions)
            {
                if (action.IsAchievableGiven(parent.state))
                {
                    Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);
                    foreach (var effect in action.effects)
                    {
                        if (!currentState.ContainsKey(effect.Key))
                            currentState.Add(effect.Key, effect.Value);
                    }
                    GOAP_Node node = new GOAP_Node(parent, parent.cost + action.cost, currentState, action);
                    if(GoalAchieved(goal, currentState))
                    {
                        leaves.Add(node);
                        foundPath = true;
                    }
                    else
                    {
                        List<GOAP_Action> subset = ActionSubset(usableActions, action);
                        bool found = BuildGraph(node, leaves, subset, goal);
                        if (found)
                            foundPath = true;
                    }
                }
            }
            return foundPath;
        }

        private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
        {
            foreach (var g in goal)
            {
                if (!state.ContainsKey(g.Key))
                    return false;
            }
            return true;
        }

        private List<GOAP_Action> ActionSubset(List<GOAP_Action> actions, GOAP_Action removeMe)
        {
            List<GOAP_Action> subset = new List<GOAP_Action>();
            foreach (var a in actions)
            {
                if (!a.Equals(removeMe))
                    subset.Add(a);
            }
            return subset;
        }

        
    } 
}
