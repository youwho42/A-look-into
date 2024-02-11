using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableMirror : Interactable
    {

        public bool isOpen;
        public override void Start()
        {
            base.Start();

        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (!isOpen)
                OpenGuiseUI();
            else
                CloseGuiseUI();

        }

        void OpenGuiseUI()
        {

            PlayInteractSound();

            UIScreenManager.instance.DisplayScreen(UIScreenType.GuiseScreen);
            //UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);

            GuiseUI.instance.ShowUI();
        }
        public void CloseGuiseUI()
        {

            UIScreenManager.instance.HideAllScreens();
            //if (LevelManager.instance.HUDBinary == 1)
            //    UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);

            GuiseUI.instance.HideUI();

        }

        void PlayInteractSound()
        {
            if (audioManager.CompareSoundNames("PickUp-" + interactSound))
            {
                audioManager.PlaySound("PickUp-" + interactSound);
            }
        }
    } 
}
