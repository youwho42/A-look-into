using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Klaxon.Interactable;


public class BallPersonMessageDisplayUI : MonoBehaviour
{
    public static BallPersonMessageDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    IBallPerson ballPerson;

    UndertakingObject undertaking;

    public TextMeshProUGUI messageTitle;
    public TextMeshProUGUI messageContent;

    bool destroyOnClose;
    InteractableFixingArea fixingArea;

    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.BallPersonDialogueUI);
    
        gameObject.SetActive(false);
    }
    public void ShowFixingAreaIngredients(InteractableFixingArea fixingArea, string messageName, string messageDescription)
    {
        //SetPlayer();
        this.fixingArea = fixingArea;
        messageTitle.text = messageName;
        messageContent.text = messageDescription;
        destroyOnClose = false;
    }

    public void ShowHowTo(string messageName, string messageDescription)
    {
        //SetPlayer();
        messageTitle.text = messageName;
        messageContent.text = messageDescription;
        destroyOnClose = false;
    }

    public void ShowBallPersonMessageUI(IBallPerson messengerAI, string messageName, string messageDescription)
    {
        //SetPlayer();
        ballPerson = messengerAI;
        messageTitle.text = messageName;
        messageContent.text = messageDescription;
        destroyOnClose = true;
    }


    public void ShowBallPersonUndertakingUI(IBallPerson _ballPerson, UndertakingObject _undertaking, bool _destroyOnClose)
    {
        //SetPlayer();
        ballPerson = _ballPerson;
        undertaking = _undertaking;
        messageTitle.text = undertaking.localizedName.GetLocalizedString();
        destroyOnClose = _destroyOnClose;
        string t = undertaking.CurrentState == UndertakingState.Complete ? undertaking.localizedCompleteDescription.GetLocalizedString() : undertaking.localizedDescription.GetLocalizedString();
        messageContent.text = t;
    }

    public void CloseMessageUI()
    {
        UIScreenManager.instance.HideScreenUI();
        //PlayerInformation.instance.playerInput.isInUI = false;
        //PlayerInformation.instance.uiScreenVisible = false;
        //PlayerInformation.instance.TogglePlayerInput(true);
        if(destroyOnClose)
            Invoke("DestroyMessenger", .1f);
        if(fixingArea != null)
        {
            fixingArea.canInteract = true;
            fixingArea = null;
        }
    }

    //void SetPlayer()
    //{
    //    PlayerInformation.instance.playerInput.isInUI = true;
    //    PlayerInformation.instance.uiScreenVisible = true;
    //    PlayerInformation.instance.TogglePlayerInput(false);
    //}

    void DestroyMessenger()
    {
        ballPerson.SetToRemoveState();
        ballPerson = null;
    }

}
