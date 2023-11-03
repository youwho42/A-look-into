using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableFarmingArea : Interactable
    {
        bool isOpen;

        public override void Start()
        {
            base.Start();

        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (!isOpen)
                OpenFarming();
            else
                CloseFarming();

        }

        private void OpenFarming()
        {
            UIScreenManager.instance.DisplayScreen(UIScreenType.CraftingStationScreen);
            UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
            isOpen = true;
        }

        private void CloseFarming()
        {
            UIScreenManager.instance.HideScreens(UIScreenType.CraftingStationScreen);
            UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
            isOpen = false;
        }
    }

}