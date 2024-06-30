using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Klaxon.StatSystem
{
	[CreateAssetMenu(menuName = "Klaxon/Stat Modifier Database", fileName = "New-Modifier-Database")]
	public class StatModifierDatabase : ScriptableObject
	{

		public List<StatModifier> modifiers;
		
		public StatModifier GetModByName(string name)
		{
			foreach (var mod in modifiers)
			{
				if(mod.ModifierName == name)
					return mod;
			}
			return null;
		}

        public void SetAllModifiers()
        {
            modifiers = Resources.LoadAll<StatModifier>("Stats/").ToList();
            modifiers = modifiers.OrderBy(x => x.name).ToList();
        }

    }

}