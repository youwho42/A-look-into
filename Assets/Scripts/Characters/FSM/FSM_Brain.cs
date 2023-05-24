using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    public class FSM_Brain : MonoBehaviour
    {
        public FSM_BaseState initialState;

        public FSM_BaseState currentState;

        private Dictionary<Type, Component> _cachedComponents = new Dictionary<Type, Component>();


        void Awake()
        {
            currentState = initialState;
        }

        void Update()
        {
            currentState.ExecuteState(this);
        }
        void LateUpdate()
        {

            currentState.LateExecuteState(this);

        }

        public T FSM_GetComponent<T>() where T : Component
        {
            if (_cachedComponents.ContainsKey(typeof(T)))
                return _cachedComponents[typeof(T)] as T;

            var component = base.GetComponent<T>();
            if (component != null)
            {
                _cachedComponents.Add(typeof(T), component);
            }
            else
            {
                component = base.GetComponentInChildren<T>();
            }
            return component;
        }

    }
}
