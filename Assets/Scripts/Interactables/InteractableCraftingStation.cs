using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Klaxon.Interactable
{
    public class InteractableCraftingStation : Interactable
    {
        bool isOpen;
        CraftingStationDisplayUI craftingDisplay;
        QI_CraftingHandler craftingHandler;
        public QI_CraftingRecipeDatabase recipeDatabase;
        public QI_Inventory selfInventory;

        QI_ItemData pickUpItem;

        public override void Start()
        {
            base.Start();
            if(TryGetComponent(out QI_Item item))
                pickUpItem = item.Data;
       
            craftingDisplay = CraftingStationDisplayUI.instance;
            craftingHandler = GetComponent<QI_CraftingHandler>();
        }


        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                OpenCrafting();
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.CraftingStationUI)
                CloseCrafting();
               
        }

        public override void LongInteract(GameObject interactor)
        {
            base.Interact(interactor);

            if (selfInventory.Stacks.Count > 0)
            {
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Container pick up"), null, 0, NotificationsType.Warning);
                return;
            }
            else if (craftingHandler.Queues.Count > 0)
            {
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Crafting pick up"), null, 0, NotificationsType.Warning);

                //NotificationManager.instance.SetNewNotification("Must not be crafting to pick up.", NotificationManager.NotificationType.Warning);
                return;
            }
            else
                PickUpCraftingStation();

        }

        void PickUpCraftingStation()
        {
            if (pickUpItem == null)
                return;


            if (PlayerInformation.instance.playerInventory.AddItem(pickUpItem, 1, false))
            {
                if (pickUpItem.placementGumption != null)
                    PlayerInformation.instance.statHandler.RemoveModifiableModifier(pickUpItem.placementGumption);

                Destroy(gameObject);
            }

        }

        private void OpenCrafting()
        {
            if(UIScreenManager.instance.DisplayIngameUI(UIScreenType.CraftingStationUI, true))
            {
                selfInventory = selfInventory != null ? selfInventory : playerInformation.playerInventory;
                craftingDisplay.ShowUI(craftingHandler, recipeDatabase, selfInventory);
            }
            
        }

        private void CloseCrafting()
        {
            UIScreenManager.instance.HideScreenUI();
            craftingDisplay.HideUI();
        }
        
    }

}