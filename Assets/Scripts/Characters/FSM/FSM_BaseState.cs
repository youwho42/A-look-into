using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    public class FSM_BaseState : ScriptableObject
    {
        public virtual void InitializeState(FSM_Brain brain) { }
        public virtual void ExecuteState(FSM_Brain brain) { }
        public virtual void LateExecuteState(FSM_Brain brain) { }
    }

}
