using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMessenger : Interactable
{
    
    public QI_ItemData messageItem;

    [Serializable]
    public enum MessageType
    {
        Note,
        Guide
    }
    public MessageType type;

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
        MessageDisplayUI.instance.ShowUI(GetComponent<MessengerAI>(), messageItem) ;
        UIScreenManager.instance.DisplayScreen(UIScreenType.Message);
        canInteract = false;
        QI_ItemDatabase database = GetCompendiumDatabase();
        
        if (!database.Items.Contains(messageItem))
        {
            database.Items.Add(messageItem);
            GameEventManager.onNoteCompediumUpdateEvent.Invoke();
            GameEventManager.onGuideCompediumUpdateEvent.Invoke();
        }

        WorldItemManager.instance.RemoveItemFromWorldItemDictionary(messageItem.Name, 1);
        yield return new WaitForSeconds(0.33f);
    }

    QI_ItemDatabase GetCompendiumDatabase()
    {
        QI_ItemDatabase database = null;
        switch (type)
        {
            case MessageType.Note:
                database = PlayerInformation.instance.playerNotesCompendiumDatabase;
                break;
            case MessageType.Guide:
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
