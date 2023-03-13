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
        //interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
        yield return new WaitForSeconds(0.33f);
        PlayInteractSound();

        if (!PlayerInformation.instance.playerNotesCompendiumDatabase.Items.Contains(messageItem))
        {
            PlayerInformation.instance.playerNotesCompendiumDatabase.Items.Add(messageItem);
            NotificationManager.instance.SetNewNotification($"{messageItem.Name} note found", NotificationManager.NotificationType.Compendium);
            GameEventManager.onNoteCompediumUpdateEvent.Invoke();
        }

        //Destroy(gameObject);
        hasInteracted = false;

        WorldItemManager.instance.RemoveItemFromWorldItemDictionary(messageItem.Name, 1);

    }

    void PlayInteractSound()
    {
        if (audioManager.CompareSoundNames("PickUp-" + interactSound))
        {
            audioManager.PlaySound("PickUp-" + interactSound);
        }


    }
}
