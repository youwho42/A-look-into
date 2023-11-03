using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.StatSystem
{
    [CreateAssetMenu(menuName = "Klaxon/Stat Changer", fileName = "New-Stat-Changer")]
    [Serializable]
    public class StatChanger : ScriptableObject
    {
        public StatObject StatToModify;
        public ModifierType ModifierType;
        public ModifierDestination ModifierDestination;
        public float ModifierAmount;
    } 
}