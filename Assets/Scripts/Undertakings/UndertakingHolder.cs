using Klaxon.GOAD;
using Klaxon.UndertakingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    [Serializable]
    public class UndertakingHolderObject
    {
        public UndertakingObject Undertaking;
        public bool hasCondition;
        [ConditionalHide("hasCondition", true)]
        public GOAD_ScriptableCondition UndertakingAvailableCondition;
    }
    public class UndertakingHolder : MonoBehaviour
    {
        public List<UndertakingHolderObject> undertakings;
    }
}
