using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{

    public class InteractableBallPeopleVillager : Interactable
    {
        public CompleteTaskObject undertaking;
        [HideInInspector]
        public bool started;
        public override void Start()
        {
            base.Start();

        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (PlayerInformation.instance.uiScreenVisible)
                return;
            StartCoroutine(InteractCo(interactor));


        }

        IEnumerator InteractCo(GameObject interactor)
        {

            PlayInteractSound();

            if (!started)
            {
                undertaking.undertaking.ActivateUndertaking();
                started = true;
            }
            

            BallPersonMessageDisplayUI.instance.ShowBallPersonUndertakingUI(GetComponent<IBallPerson>(), undertaking.undertaking, false);
            UIScreenManager.instance.DisplayIngameUI(UIScreenType.BallPersonDialogueUI, true);
            //GetComponent<SAP_Scheduler_BP>().hasInteracted = true;
            canInteract = false;
            yield return new WaitForSeconds(0.33f);
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
