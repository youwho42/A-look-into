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

    public TextMeshProUGUI messageContent;

    bool destroyOnClose;
    Interactable fixingArea;

    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.BallPersonDialogueUI);
    
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if(!UIScreenManager.instance.inMainMenu)
            CloseMessageUI();
    }
    public void ShowFixingAreaIngredients(Interactable fixingArea, string messageName, string messageDescription)
    {
        this.fixingArea = fixingArea;
        messageContent.text = $"\n<style=\"H1\">{messageName}</style>\n\n{messageDescription}\n\n";
        destroyOnClose = false;
    }

    public void ShowSimpleMessage(string messageName, string messageDescription)
    {
        messageContent.text = $"\n<style=\"H1\">{messageName}</style>\n\n{messageDescription}\n\n";
        destroyOnClose = false;
    }

    public void ShowBallPersonMessageUI(IBallPerson messengerAI, string messageName, string messageDescription)
    {
        ballPerson = messengerAI;
        messageContent.text = $"\n<style=\"H1\">{messageName}</style>\n\n{messageDescription}\n\n";
        destroyOnClose = true;
    }


    public void ShowBallPersonUndertakingUI(IBallPerson _ballPerson, UndertakingObject _undertaking, bool _destroyOnClose)
    {
        ballPerson = _ballPerson;
        undertaking = _undertaking;
        destroyOnClose = _destroyOnClose;
        string t = undertaking.CurrentState == UndertakingState.Complete ? undertaking.localizedCompleteDescription.GetLocalizedString() : undertaking.localizedDescription.GetLocalizedString();
        
        messageContent.text = $"\n<style=\"H1\">{undertaking.localizedName.GetLocalizedString()}</style>\n\n{t}\n\n";
    }

    public void CloseMessageUI()
    {
        UIScreenManager.instance.HideScreenUI();
        
        if(destroyOnClose && ballPerson != null)
            Invoke("DestroyMessenger", .1f);
        if(fixingArea != null)
        {
            fixingArea.canInteract = true;
            fixingArea = null;
        }
    }

    
    void DestroyMessenger()
    {
        if(ballPerson != null)
        {
            ballPerson.SetToRemoveState();
            ballPerson = null;
        }
            
    }

}
