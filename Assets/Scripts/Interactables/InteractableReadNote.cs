using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

public class InteractableReadNote : Interactable
{
    public QI_ItemData readableItem;

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
        interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
        yield return new WaitForSeconds(0.33f);
        PlayInteractSound();

        if (!PlayerInformation.instance.playerNotesCompendiumDatabase.Items.Contains(readableItem))
        {
            PlayerInformation.instance.playerNotesCompendiumDatabase.Items.Add(readableItem);
            NotificationManager.instance.SetNewNotification("You picked up the " + readableItem.Name + ".");
        }
        //else
        //{
        //    NotificationManager.instance.SetNewNotification("You already know the " + craftingRecipe.Name + " recipe.");
        //}


        Destroy(gameObject);
        hasInteracted = false;

        WorldItemManager.instance.RemoveItemFromWorldItemDictionary(readableItem.Name, 1);
        /*if (PlayerInformation.instance.playerInventory.AddItem(interactableItem.Data, 1))
            Destroy(gameObject);
        hasInteracted = false;

        WorldItemManager.instance.RemoveItemFromWorldItemDictionary(interactableItem.Data.Name, 1);*/


    }

    void PlayInteractSound()
    {
        if (audioManager.CompareSoundNames("PickUp-" + interactSound))
        {
            audioManager.PlaySound("PickUp-" + interactSound);
        }
    }
}
