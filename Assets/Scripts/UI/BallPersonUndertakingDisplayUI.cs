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

    public void ShowBallPersonUndertakingUI(IBallPerson _ballPerson, UndertakingObject _undertaking)
    {
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        ballPerson = _ballPerson;
        undertaking = _undertaking;
        messageTitle.text = undertaking.Name;

        string t = undertaking.CurrentState == UndertakingState.Complete ? undertaking.CompletedDescription : undertaking.Description;
        messageContent.text = t;
    }

    public void CloseMessageUI()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        
        
    }

    
}
