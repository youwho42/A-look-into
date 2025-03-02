using UnityEngine;

namespace Klaxon.Interactable
{
	public class FlumpContainerInteractor : Interactable
	{

        public FlumpOozeContainer oozeContainer;
        public override void Interact(GameObject interactor)
        {


            base.Interact(interactor);
            playerInformation.playerAnimator.SetTrigger("CleanOoze");
            oozeContainer.StartCleanOoze();
            canInteract = false;
        }

    } 
}
