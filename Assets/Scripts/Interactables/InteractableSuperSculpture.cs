using UnityEngine;
using System.Collections.Generic;


namespace Klaxon.Interactable
{
	public class InteractableSuperSculpture : Interactable
	{



        public override void Start()
        {
            base.Start();
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.SupershapeUI)
            {
                Close();
                return;
            }
            
            
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            {
                if (PlayerInformation.instance.uiScreenVisible || PlayerInformation.instance.playerInput.isInUI)
                    return;

                Open();
            }
        }


        private void Open()
        {
            UIScreenManager.instance.DisplayIngameUI(UIScreenType.SupershapeUI, true);
            
        }

        public void Close()
        {
            UIScreenManager.instance.HideScreenUI();
        }


    }

}