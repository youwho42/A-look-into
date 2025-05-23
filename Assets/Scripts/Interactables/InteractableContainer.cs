﻿using QuantumTek.QuantumInventory;
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
        public bool isLostAndFound;
        public SpriteRenderer containerSprite;
        public Sprite closedSprite;
        public Sprite openSprite;

        public override void Start()
        {
            base.Start();
            containerUI = ContainerInventoryDisplayUI.instance;
            if (isSquirrelBox)
            {
                inventory = SquirrelBoxManager.instance.inventory;
            }
            else if (isLostAndFound)
            {
                inventory = LostAndFoundManager.instance.inventory;
            }
            else
            {
                inventory = GetComponent<QI_Inventory>();

                if (inventory == null)
                    inventory = GetComponentInParent<QI_Inventory>();
            }
            if (closedSprite != null)
                containerSprite.sprite = closedSprite;
            if(inventory.Name == "EditorChest")
            {
#if UNITY_STANDALONE && !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
            }

        }


        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            {
                if (PlayerInformation.instance.uiScreenVisible || PlayerInformation.instance.playerInput.isInUI)
                    return;
                OpenContainer();
            }
            else if(UIScreenManager.instance.GetCurrentUI() == UIScreenType.ContainerUI)
            {
                CloseContainer();
            }
                
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
            if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.ContainerUI, true))
                containerUI.ShowContainerUI(inventory);
            SetContainerImage(true);
        }

        public void SetContainerImage(bool state)
        {
            if (state)
            {
                if (openSprite != null)
                    containerSprite.sprite = openSprite;
                return;
            }
            
            if (closedSprite != null)
                containerSprite.sprite = closedSprite;
        }

        private void CloseContainer()
        {
            UIScreenManager.instance.HideScreenUI();
            containerUI.HideContainerUI();
            SetContainerImage(false);
        }


        public virtual void PlayInteractionSound()
        {
            audioManager.PlaySound(interactSound);
        }


    } 
}

