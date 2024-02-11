using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableTeleporter : Interactable
    {
        Teleport teleport;
        bool isOpen;
        TeleportDisplayUI teleportUI;

        public override void Start()
        {
            base.Start();
            teleport = GetComponent<Teleport>();
            teleportUI = TeleportDisplayUI.instance;
        }
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);

            if (!isOpen)
            {
                OpenTeleporter();
                isOpen = true;
            }
            else
            {
                CloseTeleporter();
                isOpen = false;
            }

            teleport.SetUpTeleporter(interactor);
        }
        private void OpenTeleporter()
        {
            UIScreenManager.instance.DisplayScreen(UIScreenType.ContainerScreen);
            teleportUI.ShowUI(teleport);
        }

        private void CloseTeleporter()
        {
            UIScreenManager.instance.HideScreens(UIScreenType.ContainerScreen);
            //UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
            teleportUI.HideUI();
        }
    }

}