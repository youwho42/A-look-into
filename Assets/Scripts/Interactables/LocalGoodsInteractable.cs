using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGoodsInteractable : Interactable
{
    public bool isOpen;
    QI_Inventory inventory;

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
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        LocalGoodDisplayUI.instance.ShowGoodsUI(inventory);
    }

    private void CloseLocalGoods()
    {
        UIScreenManager.instance.HideAllScreens();
        if (LevelManager.instance.HUDBinary == 1)
            UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        LocalGoodDisplayUI.instance.HideGoodsUI();
    }
}
