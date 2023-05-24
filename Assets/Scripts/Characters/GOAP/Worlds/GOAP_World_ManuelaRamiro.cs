using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.GOAP
{
	public class GOAP_World_ManuelaRamiro : GOAP_World
	{
		public QI_Inventory seedBoxInventory;

        public override QI_Inventory GetWorldInventory()
        {
            return seedBoxInventory;
        }
    } 
}
