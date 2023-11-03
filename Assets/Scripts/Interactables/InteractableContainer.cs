using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;


namespace Klaxon.Interactable
{
    public class InteractableContainer : Interactable
    {

        public bool isOpen;
        ContainerInventoryDisplayUI containerUI;
        QI_Inventory inventory;
        public bool isSquirrelBox;

        public override void Start()
        {
            base.Start();
            containerUI = ContainerInventoryDisplayUI.instance;
            if (isSquirrelBox)
            {
                inventory = SquirrelBoxManager.instance.inventory;
            }
            else
            {
                inventory = GetComponent<QI_Inventory>();

                if (inventory == null)
                    inventory = GetComponentInParent<QI_Inventory>();
            }

        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);

            if (!isOpen)
                OpenContainer();
            else
                CloseContainer();
        }

        public override void LongInteract(GameObject interactor)
        {
            base.Interact(interactor);

            if (inventory.Stacks.Count > 0)
            {
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Container pick up"), null, 0, NotificationsType.Warning);
                return;
            }
            else
                PickUpContainer();

        }

        void PickUpContainer()
        {
            var item = GetComponent<QI_Item>().Data;
            if (PlayerInformation.instance.playerInventory.AddItem(item, 1, false))
            {
                Notifications.instance.SetNewNotification("", item, 1, NotificationsType.Inventory);

                Destroy(gameObject);
            }

        }

        private void OpenContainer()
        {
            UIScreenManager.instance.DisplayScreen(UIScreenType.ContainerScreen);
            UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
            containerUI.ShowContainerUI(inventory);
            isOpen = true;
        }

        private void CloseContainer()
        {
            UIScreenManager.instance.HideAllScreens();
            if (LevelManager.instance.HUDBinary == 1)
                UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
            containerUI.HideContainerUI();
            isOpen = false;
        }


        public virtual void PlayInteractionSound()
        {
            audioManager.PlaySound(interactSound);
        }
    } 
}

