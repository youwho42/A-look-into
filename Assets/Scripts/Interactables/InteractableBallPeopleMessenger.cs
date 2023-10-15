using Klaxon.SAP;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum BallPeopleMessageType
{
    Note,
    Guide
}
public class InteractableBallPeopleMessenger : Interactable
{
    
    public QI_ItemData messageItem;
    public BallPeopleMessageType type;
    public UndertakingObject undertaking;
    public QI_CraftingRecipe craftingRecipe;

    public override void Start()
    {
        base.Start();

    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (PlayerInformation.instance.uiScreenVisible)
            return;

        StartCoroutine(InteractCo(interactor));


    }

    IEnumerator InteractCo(GameObject interactor)
    {
        
        PlayInteractSound();
        var messenger = GetComponent<SAP_Scheduler_BP>();

        if (messageItem != null)
        {

            BallPersonMessageDisplayUI.instance.ShowBallPersonMessageUI(messenger, messageItem.localizedName.GetLocalizedString(), messageItem.localizedDescription.GetLocalizedString());
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
            BallPersonMessageDisplayUI.instance.ShowBallPersonMessageUI(messenger, undertaking.localizedName.GetLocalizedString(), undertaking.localizedDescription.GetLocalizedString());
            
            PlayerInformation.instance.playerUndertakings.AddUndertaking(undertaking);
        }
        else if (craftingRecipe != null)
        {
            string desc = "";
            for (int i = 0; i < craftingRecipe.Ingredients.Count; i++)
            {
                desc += $"{craftingRecipe.Ingredients[i].Amount} - {craftingRecipe.Ingredients[i].Item.Name}\n";

            }
            BallPersonMessageDisplayUI.instance.ShowBallPersonMessageUI(messenger, craftingRecipe.Name, desc);
            PlayerInformation.instance.playerRecipeDatabase.CraftingRecipes.Add(craftingRecipe);
        }

        // display recipe name and ingredients...
        // add recipe to player recipes
        UIScreenManager.instance.DisplayScreen(UIScreenType.BallPersonUndertakingScreen);
        GetComponent<SAP_Scheduler_BP>().hasInteracted = true;
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
