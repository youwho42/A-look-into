using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (UIScreenManager.instance.CurrentUIScreen() == UIScreenType.None)
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
        //UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        ResearchStationDisplayUI.instance.HideUI();
    }
   

}
