using Klaxon.GOAD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Klaxon/Instruction Object", fileName = "New Instruction")]
public class InstructionObject : ScriptableObject
{
    public GOAD_ScriptableCondition condition;
    public LocalizedString title;
    public LocalizedString description;
}
