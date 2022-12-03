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
            if (UIScreenManager.instance.CurrentUIScreen() == UIScreenType.PlayerUI)
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
        ResearchStationDisplayUI.instance.ShowUI();
    }

    private void CloseResearch()
    {
        UIScreenManager.instance.HideScreens(UIScreenType.ResearchStationScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        ResearchStationDisplayUI.instance.HideUI();
    }
   

}
