using Klaxon.GOAD;
using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableBallPeopleSeeker : Interactable
    {
        public CompleteTaskObject talkTask;
        //public UndertakingObject undertaking;
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
            bool destroyOnClose = false;
            
            if (started)
            {
                destroyOnClose = true;
                talkTask.undertaking.TryCompleteTask(talkTask.task);
            }

            BallPersonMessageDisplayUI.instance.ShowBallPersonUndertakingUI(GetComponent<IBallPerson>(), talkTask.undertaking, destroyOnClose, this);
            UIScreenManager.instance.DisplayIngameUI(UIScreenType.BallPersonDialogueUI, true);
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
        public override void SetGuideOrNote()
        {
            if (!started)
            {
                talkTask.undertaking.ActivateUndertaking();
                started = true;
            }
        }
    } 
}