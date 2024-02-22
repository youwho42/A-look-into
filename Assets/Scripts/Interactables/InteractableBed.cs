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

        public override void Start()
        {
            base.Start();
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