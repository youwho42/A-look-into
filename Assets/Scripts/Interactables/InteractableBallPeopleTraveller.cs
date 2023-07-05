using Klaxon.SAP;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBallPeopleTraveller : Interactable
{
    public CompleteTaskObject undertaking;
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

        if (!started)
        {
            undertaking.undertaking.ActivateUndertaking();
            started = true;
        }
        else
        {
            undertaking.undertaking.TryCompleteTask(undertaking.task);
        }
        
        BallPersonMessageDisplayUI.instance.ShowBallPersonUndertakingUI(GetComponent<IBallPerson>(), undertaking.undertaking, undertaking.undertaking.CurrentState == UndertakingState.Complete);
        UIScreenManager.instance.DisplayScreen(UIScreenType.BallPersonUndertakingScreen);
        GetComponent<SAP_Scheduler_BP>().hasInteracted = true;
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
