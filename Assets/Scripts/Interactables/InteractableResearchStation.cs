using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableResearchStation : Interactable
    {

        bool isOpen;
        //ResearchStationDisplayUI researchDisplay;
        public override void Start()
        {
            base.Start();
            //researchDisplay = ResearchStationDisplayUI.instance;
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (!isOpen)
            {
                if (PlayerInformation.instance.uiScreenVisible || PlayerInformation.instance.playerInput.isInUI)
                    return;
                var screen = LevelManager.instance.HUDBinary == 0 ? UIScreenType.None : UIScreenType.PlayerUI;
                if (UIScreenManager.instance.CurrentUIScreen() == screen)
                {
                    OpenResearch();
                    isOpen = true;
                }

            }
            else
            {
                CloseResearch();
                isOpen = false;
            }
        }

        private void OpenResearch()
        {
            UIScreenManager.instance.DisplayScreen(UIScreenType.ResearchStationScreen);
            UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
            ResearchStationDisplayUI.instance.ShowUI();
        }

        private void CloseResearch()
        {
            UIScreenManager.instance.HideAllScreens();
            if (LevelManager.instance.HUDBinary == 1)
                UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
            ResearchStationDisplayUI.instance.HideUI();
        }


    }

}