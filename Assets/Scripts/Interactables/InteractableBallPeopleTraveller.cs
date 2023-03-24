using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBallPeopleTraveller : Interactable
{

    public QI_ItemData messageItem;
    public BallPeopleMessageType type;
    public UndertakingObject undertaking;

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

        if (messageItem != null)
        {
            MessageDisplayUI.instance.ShowUI(GetComponent<BallPeopleMessengerAI>(), messageItem.Name, messageItem.Description);
            QI_ItemDatabase database = GetCompendiumDatabase();

            if (!database.Items.Contains(messageItem))
            {
                database.Items.Add(messageItem);
                GameEventManager.onNoteCompediumUpdateEvent.Invoke();
                GameEventManager.onGuideCompediumUpdateEvent.Invoke();
            }
        }
        else if (undertaking != null)
        {
            MessageDisplayUI.instance.ShowUI(GetComponent<BallPeopleMessengerAI>(), undertaking.Name, undertaking.Description);

            PlayerInformation.instance.playerUndertakings.AddUndertaking(undertaking);
        }
        UIScreenManager.instance.DisplayScreen(UIScreenType.Message);
        GetComponent<BallPeopleMessengerAI>().hasInteracted = true;
        canInteract = false;
        //WorldItemManager.instance.RemoveItemFromWorldItemDictionary(messageItem.Name, 1);
        yield return new WaitForSeconds(0.33f);
    }

    QI_ItemDatabase GetCompendiumDatabase()
    {
        QI_ItemDatabase database = null;
        switch (type)
        {
            case BallPeopleMessageType.Note:
                database = PlayerInformation.instance.playerNotesCompendiumDatabase;
                break;
            case BallPeopleMessageType.Guide:
                database = PlayerInformation.instance.playerGuidesCompendiumDatabase;
                break;

        }
        return database;
    }

    void PlayInteractSound()
    {
        if (audioManager.CompareSoundNames("PickUp-" + interactSound))
        {
            audioManager.PlaySound("PickUp-" + interactSound);
        }


    }
}
