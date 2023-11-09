using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Klaxon.Interactable
{
	public class InteractableDecoration : Interactable
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
            
            
            if (PlayerInformation.instance.playerInventory.AddItem(pickUpItem, 1, false))
            {
                if (replaceObjectOnDrop != null)
                    replaceObjectOnDrop.ShowObjects(true);

                PlayerInformation.instance.statHandler.RemoveModifiableModifier(pickUpItem.placementGumption);

                Destroy(gameObject);
            }
        }

    }

}