using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
	public class InteractableDecorationDestructable : Interactable
    {
        QI_ItemData pickUpItem;

        public override void Start()
        {
            base.Start();
            pickUpItem = GetComponent<QI_Item>().Data;
        }

        public override void SetInteractVerb()
        {
            interactVerb = "";
        }

        public override void LongInteract(GameObject interactor)
        {

            base.LongInteract(interactor);

            

            PlayerInformation.instance.statHandler.RemoveModifiableModifier(pickUpItem.placementGumption);
            Destroy(gameObject);
            
        }
    }
}
