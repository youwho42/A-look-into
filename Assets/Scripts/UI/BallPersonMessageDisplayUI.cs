using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Klaxon.Interactable;
using QuantumTek.QuantumInventory;

public class BallPersonMessageDisplayUI : MonoBehaviour
{
    public static BallPersonMessageDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    IBallPerson ballPerson;
    Interactable currentInteractable;
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

    public void ShowSimpleMessage(string messageName, string messageDescription, Interactable interactableBP)
    {
        currentInteractable = interactableBP;
        messageContent.text = $"\n<style=\"H1\">{messageName}</style>\n\n{messageDescription}\n\n";
        destroyOnClose = false;
    }

    public void ShowBallPersonMessageUI(IBallPerson messengerAI, QI_ItemData messageItem, Interactable interactableBP)
    {
        currentInteractable = interactableBP;
        ballPerson = messengerAI;
        messageContent.text = $"\n<style=\"H1\">{messageItem.localizedName.GetLocalizedString()}</style>\n\n{messageItem.localizedDescription.GetLocalizedString()}\n\n";
        destroyOnClose = true;
    }
    public void ShowBallPersonMessageUI(IBallPerson messengerAI, UndertakingObject messageItem, Interactable interactableBP)
    {
        currentInteractable = interactableBP;
        ballPerson = messengerAI;
        messageContent.text = $"\n<style=\"H1\">{messageItem.localizedName.GetLocalizedString()}</style>\n\n{messageItem.localizedDescription.GetLocalizedString()}\n\n";
        destroyOnClose = true;
    }
    public void ShowBallPersonMessageUI(IBallPerson messengerAI, QI_CraftingRecipe messageItem, Interactable interactableBP)
    {
        currentInteractable = interactableBP;
        string desc = "";
        for (int i = 0; i < messageItem.Ingredients.Count; i++)
        {
            desc += $"{messageItem.Ingredients[i].Amount} - {messageItem.Ingredients[i].Item.localizedName.GetLocalizedString()}\n";

        }
        ballPerson = messengerAI;
        messageContent.text = $"\n<style=\"H1\">{messageItem.Product.Item.localizedName.GetLocalizedString()}</style>\n\n{desc}\n\n";
        destroyOnClose = true;
    }

    public void ShowBallPersonUndertakingUI(IBallPerson _ballPerson, UndertakingObject _undertaking, bool _destroyOnClose, Interactable interactableBP)
    {
        currentInteractable = interactableBP;
        ballPerson = _ballPerson;
        undertaking = _undertaking;
        destroyOnClose = _destroyOnClose;
        string t = undertaking.CurrentState == UndertakingState.Complete ? undertaking.localizedCompleteDescription.GetLocalizedString() : undertaking.localizedDescription.GetLocalizedString();
        
        messageContent.text = $"\n<style=\"H1\">{undertaking.localizedName.GetLocalizedString()}</style>\n\n{t}\n\n";
    }

    public void CloseMessageUI()
    {
        if (currentInteractable != null)
            currentInteractable.SetGuideOrNote();
        UIScreenManager.instance.HideScreenUI();
        if(ballPerson != null)
            ballPerson.InteractionFinished();
        if(destroyOnClose && ballPerson != null)
            Invoke("DestroyMessenger", .1f);
        if(fixingArea != null)
        {
            fixingArea.canInteract = true;
            fixingArea = null;
        }
        currentInteractable = null;
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
