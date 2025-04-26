using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractablePainting : Interactable
    {
        bool isOpen;
        public RestorePainting painting;
        public RestoreSculpture sculpture;
        

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.PaintingUI)
            {
                Close();
            }
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.RestorePaintingUI)
            {
                Close();
            }
            if (onlyLongInteract)
                return;
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            {
                if (PlayerInformation.instance.uiScreenVisible || PlayerInformation.instance.playerInput.isInUI)
                    return;

                OpenPainting();
            }
            
        }

        public override void LongInteract(GameObject interactor)
        {
            base.LongInteract(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            {
                if (PlayerInformation.instance.uiScreenVisible || PlayerInformation.instance.playerInput.isInUI)
                    return;

                OpenRestorePainting();
            }
            
        }

        private void OpenPainting()
        {
            if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.PaintingUI, true))
                PaintingDisplayUI.instance.ShowUI(painting);
                
        }
        private void OpenRestorePainting()
        {
            if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.RestorePaintingUI, true))
            {
                if (painting != null)
                    PaintingRestorationUI.instance.ShowUI(painting);
                else if (sculpture != null)
                    PaintingRestorationUI.instance.ShowUI(sculpture);


            }
        }

        public void Close()
        {
            UIScreenManager.instance.HideScreenUI();
        }

    }
}

