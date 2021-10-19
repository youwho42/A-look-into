using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableResearchStation : Interactable
{

    bool isOpen;
    ResearchStationDisplayUI researchDisplay;

    public override void Start()
    {
        base.Start();
        researchDisplay = ResearchStationDisplayUI.instance;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (!isOpen)
        {
            OpenResearch();
            isOpen = true;
        }
        else
        {
            CloseResearch();
            isOpen = false;
        }
    }

    private void OpenResearch()
    {
        researchDisplay.ShowReasearchStationUI();
    }

    private void CloseResearch()
    {
        
        researchDisplay.HideReasearchStationUI();
    }
   

}
