using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAP
{
	public abstract class GOAP_World : MonoBehaviour
	{
		private GOAP_WorldStates world;

		
		void Start()
		{
			world = new GOAP_WorldStates();
		}


		public GOAP_WorldStates GetWorld()
		{
			return world;
		}

		public abstract QI_Inventory GetWorldInventory();
	} 
}
