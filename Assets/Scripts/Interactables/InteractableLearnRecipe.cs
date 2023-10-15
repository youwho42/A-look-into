using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

public class InteractableLearnRecipe : Interactable
{
    public QI_CraftingRecipe craftingRecipe;

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

        //if (PlayerCrafting.instance.AddCraftingRecipe(craftingRecipe))
        //{
        //    NotificationManager.instance.SetNewNotification("You learned the " + craftingRecipe.Name + " recipe.");
        //}
        //else
        //{
        //    NotificationManager.instance.SetNewNotification("You already know the " + craftingRecipe.Name + " recipe.");
        //}
    

        Destroy(gameObject);
        hasInteracted = false;

        WorldItemManager.instance.RemoveItemFromWorldItemDictionary(craftingRecipe.Name, 1);
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
