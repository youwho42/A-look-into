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
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                OpenLocalGoods();
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.LocalGoodsUI)
                CloseLocalGoods();
        }

        private void OpenLocalGoods()
        {
            UIScreenManager.instance.DisplayIngameUI(UIScreenType.LocalGoodsUI, true);
            LocalGoodDisplayUI.instance.ShowGoodsUI(inventory, validType, priceMultiplier, localizedShopName.GetLocalizedString());
        }

        private void CloseLocalGoods()
        {
            LocalGoodDisplayUI.instance.HideGoodsUI();
            UIScreenManager.instance.HideScreenUI();
            
        }
    } 
}
