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
        public override void Start()
        {
            base.Start();
            
            craftingDisplay = CraftingStationDisplayUI.instance;
            craftingHandler = GetComponent<QI_CraftingHandler>();
        }


        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            {
                if (PlayerInformation.instance.uiScreenVisible || PlayerInformation.instance.playerInput.isInUI)
                    return;
                
                OpenCrafting();
                
                
            }
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.CraftingStationUI)
            {
                CloseCrafting();
               
            }
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
            var item = GetComponent<QI_Item>().Data;
            if (PlayerInformation.instance.playerInventory.AddItem(item, 1, false))
            {
                if (replaceObjectOnDrop != null)
                    replaceObjectOnDrop.ShowObjects(true);
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