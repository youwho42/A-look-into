using UnityEngine;
using System.Collections.Generic;
using Klaxon.Interactable;

public class HammockInteractable : Interactable
{

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            ViewSky();
        else if(UIScreenManager.instance.GetCurrentUI() == UIScreenType.Sky)
            HideSky();
    }

    private void ViewSky()
    {
        UIScreenManager.instance.DisplayIngameUI(UIScreenType.Sky, true);
    }

    
    private void HideSky()
    {
        UIScreenManager.instance.HideScreenUI();
    }

}
