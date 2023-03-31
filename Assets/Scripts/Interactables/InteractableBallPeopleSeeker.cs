using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        StartCoroutine(InteractCo(interactor));


    }

    IEnumerator InteractCo(GameObject interactor)
    {

        PlayInteractSound();
        bool destroyOnClose = false;
        if (!started)
        {
            talkTask.undertaking.ActivateUndertaking();
            started = true;
        }
        else
        {
            destroyOnClose = true;
            talkTask.undertaking.TryCompleteTask(talkTask.task);
        }

        BallPersonUndertakingDisplayUI.instance.ShowBallPersonUndertakingUI(GetComponent<IBallPerson>(), talkTask.undertaking, destroyOnClose);
        UIScreenManager.instance.DisplayScreen(UIScreenType.BallPersonUndertakingScreen);
        GetComponent<BallPeopleSeekerAI>().hasInteracted = true;
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