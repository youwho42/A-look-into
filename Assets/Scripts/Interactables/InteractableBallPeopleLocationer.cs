using Klaxon.GOAD;
using Klaxon.UndertakingSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;

namespace Klaxon.Interactable
{
	public class InteractableBallPeopleLocationer : Interactable
    {

        public GOAD_ScriptableCondition conditionForLocationer;
        public CompleteTaskObject talkTask;
        public LocalizedString messageTitle;
        public LocalizedString messageDescription;
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
            var bp = GetComponent<GOAD_Scheduler_BP>();
            PlayInteractSound();
            
            if (!started)
            {
                bp.SetBeliefState(conditionForLocationer.Condition, conditionForLocationer.State);
                talkTask.undertaking.ActivateUndertaking();
                started = true;
            }
           

            BallPersonMessageDisplayUI.instance.ShowSimpleMessage(messageTitle.GetLocalizedString(), messageDescription.GetLocalizedString());
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


    }

}