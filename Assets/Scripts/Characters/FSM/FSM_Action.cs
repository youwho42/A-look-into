using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    public abstract class FSM_Action : ScriptableObject
    {
        public abstract void ResetAction(FSM_Brain brain);
        public abstract void ExecuteAction(FSM_Brain brain);
        public abstract void LateExecuteAction(FSM_Brain brain);
    }

}
