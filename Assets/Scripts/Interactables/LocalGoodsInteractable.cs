using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Klaxon.Interactable
{
    public class LocalGoodsInteractable : Interactable
    {
        public bool isOpen;

        QI_Inventory inventory;
        public string shopName;
        public LocalizedString localizedShopName;
        public ItemType validType;
        [Range(1.0f, 1.5f)]
        public float priceMultiplier;
        public override void Start()
        {
            base.Start();
            inventory = GetComponent<QI_Inventory>();
        }
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (!isOpen)
            {
                var screen = LevelManager.instance.HUDBinary == 0 ? UIScreenType.None : UIScreenType.PlayerUI;
                if (UIScreenManager.instance.CurrentUIScreen() == screen)
                {
                    OpenLocalGoods();
                    isOpen = true;
                }

            }
            else
            {
                CloseLocalGoods();
                isOpen = false;
            }
        }

        private void OpenLocalGoods()
        {
            UIScreenManager.instance.DisplayScreen(UIScreenType.LocalGoods);
            UIScreenManager.instance.DisplayPlayerHUD(true);
            LocalGoodDisplayUI.instance.ShowGoodsUI(inventory, validType, priceMultiplier, localizedShopName.GetLocalizedString());
        }

        private void CloseLocalGoods()
        {
            UIScreenManager.instance.HideAllScreens();
            UIScreenManager.instance.DisplayPlayerHUD(LevelManager.instance.HUDBinary == 1);
            LocalGoodDisplayUI.instance.HideGoodsUI();
        }
    } 
}
