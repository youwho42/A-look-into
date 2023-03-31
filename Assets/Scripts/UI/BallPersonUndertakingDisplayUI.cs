using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BallPersonUndertakingDisplayUI : MonoBehaviour
{
    public static BallPersonUndertakingDisplayUI instance;

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

    
    
    public void ShowBallPersonMessageUI(IBallPerson messengerAI, string messageName, string messageDescription)
    {
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        ballPerson = messengerAI;
        messageTitle.text = messageName;
        messageContent.text = messageDescription;
        destroyOnClose = true;
    }


    public void ShowBallPersonUndertakingUI(IBallPerson _ballPerson, UndertakingObject _undertaking, bool _destroyOnClose)
    {
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        ballPerson = _ballPerson;
        undertaking = _undertaking;
        messageTitle.text = undertaking.Name;
        destroyOnClose = _destroyOnClose;
        string t = undertaking.CurrentState == UndertakingState.Complete ? undertaking.CompletedDescription : undertaking.Description;
        messageContent.text = t;
    }

    public void CloseMessageUI()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        if(destroyOnClose)
            Invoke("DestroyMessenger", .1f);
    }

    void DestroyMessenger()
    {
        ballPerson.SetToRemoveState();
        ballPerson = null;
    }

}
