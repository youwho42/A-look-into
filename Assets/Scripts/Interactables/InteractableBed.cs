using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableBed : Interactable
    {
        bool isOpen;
        public bool facingRight;
        public NavigationNode navigationNode;
        SleepDisplayUI sleepDisplay;
        QI_ItemData pickUpItem;

        public override void Start()
        {
            base.Start();
            pickUpItem = GetComponent<QI_Item>().Data;
            sleepDisplay = SleepDisplayUI.instance;
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                OpenSleeping();
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.SleepUI && !UIScreenManager.instance.isSleeping)
                CloseSleeping();

            

        }

        public override void LongInteract(GameObject interactor)
        {
            base.Interact(interactor);

            
            PickUpBed();

        }

        void PickUpBed()
        {
            var item = GetComponent<QI_Item>().Data;
            if (PlayerInformation.instance.playerInventory.AddItem(item, 1, false))
            {
                if (pickUpItem.placementGumption != null)
                    PlayerInformation.instance.statHandler.RemoveModifiableModifier(pickUpItem.placementGumption);
                if (replaceObjectOnDrop != null)
                    replaceObjectOnDrop.ShowObjects(true);
                Destroy(gameObject);
            }

        }

        private void OpenSleeping()
        {
            if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.SleepUI, true))
                sleepDisplay.ShowUI();
        }

        private void CloseSleeping()
        {
            UIScreenManager.instance.HideScreenUI();
        }


    }

}