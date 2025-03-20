using UnityEngine;

namespace Klaxon.Interactable
{
	public class FlumpContainerInteractor : Interactable
	{

        public FlumpOozeContainer oozeContainer;
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            
            if (playerInformation.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("CleanOoze"))
                return;

            playerInformation.playerAnimator.SetTrigger("CleanOoze");
            oozeContainer.StartCleanOoze();
            canInteract = false;
        }

    } 
}
