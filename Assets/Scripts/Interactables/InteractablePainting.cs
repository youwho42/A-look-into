using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractablePainting : Interactable
    {
        bool isOpen;
        //ResearchStationDisplayUI researchDisplay;
        public RestorePainting painting;

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            {
                if (PlayerInformation.instance.uiScreenVisible || PlayerInformation.instance.playerInput.isInUI)
                    return;

                OpenPainting();
            }
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.PaintingUI)
            {
                Close();
            }
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.RestorePaintingUI)
            {
                Close();
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
                PaintingRestorationUI.instance.ShowUI(painting);
        }

        public void Close()
        {
            UIScreenManager.instance.HideScreenUI();
            PaintingDisplayUI.instance.HideUI();
            PaintingRestorationUI.instance.HideUI();
        }

    }
}

