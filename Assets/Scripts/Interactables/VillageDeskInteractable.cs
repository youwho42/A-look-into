using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class VillageDeskInteractable : Interactable
    {
        FixVillageDesk villageDesk;
        public bool isOpen;


        public override void Start()
        {
            base.Start();
            villageDesk = GetComponent<FixVillageDesk>();
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (!isOpen)
            {
                var screen = LevelManager.instance.HUDBinary == 0 ? UIScreenType.None : UIScreenType.PlayerUI;
                if (UIScreenManager.instance.CurrentUIScreen() == screen)
                {
                    OpenVillageDesk();
                    isOpen = true;
                }

            }
            else
            {
                CloseVillageDesk();
                isOpen = false;
            }
        }

        private void OpenVillageDesk()
        {
            UIScreenManager.instance.DisplayScreen(UIScreenType.VillageDesk);
            UIScreenManager.instance.DisplayPlayerHUD(true);
            VillageDeskDisplayUI.instance.ShowUI(villageDesk);
        }

        private void CloseVillageDesk()
        {
            UIScreenManager.instance.HideAllScreens();
            UIScreenManager.instance.DisplayPlayerHUD(LevelManager.instance.HUDBinary == 1);
            VillageDeskDisplayUI.instance.HideUI();
        }

    }

}