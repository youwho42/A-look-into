using Klaxon.GOAD;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace Klaxon.Interactable
{

    [Serializable]
    public class FixableAreaIngredient
    {
        public QI_ItemData item;
        public int amount;
    }

    public class InteractableFixingArea : Interactable
    {

        public List<FixableAreaIngredient> ingredients = new List<FixableAreaIngredient>();
        FixingSounds fixSound;

        public override void Start()
        {
            base.Start();
            fixSound = GetComponentInChildren<FixingSounds>();
        }

        public override void Interact(GameObject interactor)
        {
            
            
            base.Interact(interactor);
            bool hasIngredients = CheckForIngredients();
            bool cost = InteractCostReward();

            if (hasIngredients && cost)
            {
                if (InteractBounceCost())
                {
                    if (GetComponent<IFixArea>().Fix(ingredients))
                    {
                        SetGuideOrNote();
                        fixSound.StartSoundsWithTimer();
                        canInteract = false;
                        PlayerInformation.instance.statHandler.ChangeStat(bounceCost);
                    }

                }

            }
        }
        public override void LongInteract(GameObject interactor)
        {
            
            base.LongInteract(interactor);
            string ingredients = "";
            if (agencyCost > 0)
                ingredients += $"<sprite name=\"Agency\"> - {agencyCost}\n";
            for (int i = 0; i < this.ingredients.Count; i++)
            {
                ingredients += $"{this.ingredients[i].amount} - {this.ingredients[i].item.localizedName.GetLocalizedString()}\n";
            }

            BallPersonMessageDisplayUI.instance.ShowFixingAreaIngredients(this, longInteractVerb.GetLocalizedString(), ingredients);
            UIScreenManager.instance.DisplayIngameUI(UIScreenType.BallPersonDialogueUI, true);
            canInteract = false;
        }

        


        public bool CheckForIngredients()
        {
            bool hasAll = true;
            foreach (var ingredient in ingredients)
            {

                int t = PlayerInformation.instance.GetTotalInventoryQuantity(ingredient.item);
                if (t < ingredient.amount)
                {
                    Notifications.instance.SetNewNotification($"{ingredient.amount - t} {ingredient.item.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Warning);
                    hasAll = false;
                }

            }
            return hasAll;
        }

        bool InteractCostReward()
        {
            float agency = PlayerInformation.instance.statHandler.GetStatMaxModifiedValue("Agency");
            if (agency >= agencyCost)
                return true;

            Notifications.instance.SetNewNotification($"{agencyCost} <sprite name=\"Agency\">", null, 0, NotificationsType.Warning);
            return false;
        }


        
    }

}