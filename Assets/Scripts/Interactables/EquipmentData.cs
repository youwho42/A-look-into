using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/EquipmentItem", fileName = "New Equipment Item")]
public class EquipmentData : QI_ItemData
{

    public EquipmentSlot equipmentSlot;

    public EquipmentTier equipmentTier;

    public StatChanger statChanger;

    public override void UseItem()
    {
        base.UseItem();
        EquipItem();
    }
    public virtual void EquipItem()
    {
        
        var pInfo = PlayerInformation.instance;
        var eManager = EquipmentManager.instance;
        if (eManager.currentEquipment[(int)equipmentSlot] != null)
        {
            pInfo.playerInventory.RemoveItem(this, 1);
            if (eManager.UnEquipToInventory(eManager.currentEquipment[(int)equipmentSlot], (int)equipmentSlot))
            {
                eManager.Equip(this, (int)equipmentSlot);
            }
            else
            {
                pInfo.playerInventory.AddItem(this, 1, false);
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", "Inventory Full"), null, 0, NotificationsType.Warning);
            }
        } 
        else
        {
            pInfo.playerInventory.RemoveItem(this, 1);
            eManager.Equip(this, (int)equipmentSlot);
        }

    }
    public override void UseEquippedItem()
    {
        base.UseEquippedItem();
    }

    public bool InteractCostReward()
    {
        var pInfo = PlayerInformation.instance;
        if (pInfo.statHandler.GetStatCurrentModifiedValue("Bounce") >= Mathf.Abs(statChanger.Amount))
        {
            pInfo.statHandler.ChangeStat(statChanger);
            return true;
        }
        pInfo.playerAnimator.SetBool("UseEquipement", false);
        Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Missing bounce"), null, 0, NotificationsType.Warning);

        return false;
    }
}
public enum EquipmentSlot 
{ 
    Hands, 
    Light,
    Compass,
    Backpack
}

public enum EquipmentTier
{
    I,
    II,
    III,
    IV
}
