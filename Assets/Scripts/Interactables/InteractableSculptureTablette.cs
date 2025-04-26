using UnityEngine;

namespace Klaxon.Interactable
{
	public class InteractableSculptureTablette : Interactable
	{
        public SculptureSO sculpture;
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            {
                BallPersonMessageDisplayUI.instance.ShowSimpleMessage(sculpture.localizedName.GetLocalizedString(), sculpture.localizedDescription.GetLocalizedString());
                UIScreenManager.instance.DisplayIngameUI(UIScreenType.BallPersonDialogueUI, true);
            }

        }

    } 
}
