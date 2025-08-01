using Klaxon.GOAD;
//using Klaxon.SAP;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
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
            var messenger = GetComponent<GOAD_Scheduler_BP>();

            if (messageItem != null)
                BallPersonMessageDisplayUI.instance.ShowBallPersonMessageUI(messenger, messageItem, this);
                
            
            if (undertaking != null)
                BallPersonMessageDisplayUI.instance.ShowBallPersonMessageUI(messenger, undertaking, this);

            if (craftingRecipe != null)
                BallPersonMessageDisplayUI.instance.ShowBallPersonMessageUI(messenger, craftingRecipe, this);
               
            UIScreenManager.instance.DisplayIngameUI(UIScreenType.BallPersonDialogueUI, true);
            canInteract = false;
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
        public override void SetGuideOrNote()
        {
            if (messageItem != null) 
            {
                QI_ItemDatabase database = GetCompendiumDatabase();
                if (!database.Items.Contains(messageItem))
                {
                    database.Items.Add(messageItem);
                    Notifications.instance.SetNewLargeNotification(null, messageItem, null, NotificationsType.Compendium);
                    GameEventManager.onNoteCompediumUpdateEvent.Invoke();
                    GameEventManager.onGuideCompediumUpdateEvent.Invoke();
                }
            }

            if (undertaking != null)
                PlayerInformation.instance.playerUndertakings.AddUndertaking(undertaking);

            if (craftingRecipe != null)
                PlayerCrafting.instance.AddCraftingRecipe(craftingRecipe, false);

        }
    } 
}
