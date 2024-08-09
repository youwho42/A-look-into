using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.GOAD
{
    [CreateAssetMenu(menuName = "Klaxon/GOAD/Condition", fileName = "New_Condition")]
    public class GOAD_ScriptableCondition : ScriptableObject
	{

		public string Condition;
		public bool State;

	} 
}
