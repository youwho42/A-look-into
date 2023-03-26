using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageDisplayUI : MonoBehaviour
{
    public static MessageDisplayUI instance;

    IBallPerson messenger;
    

    public TextMeshProUGUI messageTitle;
    public TextMeshProUGUI messageContent;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void ShowMessengerUI(IBallPerson messengerAI, string messageName, string messageDescription)
    {
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        messenger = messengerAI;
        messageTitle.text = messageName;
        messageContent.text = messageDescription;
    }

    public void CloseMessageUI()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        Invoke("DestroyMessenger", .1f);
    }

    void DestroyMessenger()
    {
        messenger.SetToRemoveState();
        messenger = null;
    }

}
