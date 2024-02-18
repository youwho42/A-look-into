using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableMirror : Interactable
    {

        public override void Start()
        {
            base.Start();

        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                OpenGuiseUI();
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.GuiseUI)
                CloseGuiseUI();

        }

        void OpenGuiseUI()
        {
            PlayInteractSound();
            UIScreenManager.instance.DisplayIngameUI(UIScreenType.GuiseUI, true);
            GuiseUI.instance.ShowUI();
        }
        public void CloseGuiseUI()
        {
            GuiseUI.instance.HideUI();
            UIScreenManager.instance.HideScreenUI();
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
