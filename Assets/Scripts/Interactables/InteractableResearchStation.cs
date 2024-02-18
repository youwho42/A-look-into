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
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            {
                if (PlayerInformation.instance.uiScreenVisible || PlayerInformation.instance.playerInput.isInUI)
                    return;
                
                OpenResearch();
            }
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.ResearchStationUI)
            {
                CloseResearch();
            }
        }

        private void OpenResearch()
        {
            if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.ResearchStationUI, true))
                ResearchStationDisplayUI.instance.ShowUI();
        }

        private void CloseResearch()
        {
            UIScreenManager.instance.HideScreenUI();
            ResearchStationDisplayUI.instance.HideUI();
        }


    }

}