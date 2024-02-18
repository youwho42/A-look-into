using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class VillageDeskInteractable : Interactable
    {
        FixVillageDesk villageDesk;


        public override void Start()
        {
            base.Start();
            villageDesk = GetComponent<FixVillageDesk>();
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                OpenVillageDesk();
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.VillageDeskUI)
                CloseVillageDesk();
        }

        private void OpenVillageDesk()
        {
            UIScreenManager.instance.DisplayIngameUI(UIScreenType.VillageDeskUI, true);
            
            VillageDeskDisplayUI.instance.ShowUI(villageDesk);
        }

        private void CloseVillageDesk()
        {
            UIScreenManager.instance.HideScreenUI();
            VillageDeskDisplayUI.instance.HideUI();
        }

    }

}