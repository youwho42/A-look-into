using UnityEngine;
using UnityEngine.Localization.Settings;


[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/Equipment Item/BackpackItem", fileName = "New Backpack Item")]
public class EquipmentBackpackData : EquipmentData
{

    public int additionalSlots;

    public override void EquipItem()
    {
        var pInfo = PlayerInformation.instance;
        var eManager = EquipmentManager.instance;
        if (eManager.currentEquipment[(int)equipmentSlot] != null)
        {
            var equipedBackpack = eManager.currentEquipment[(int)equipmentSlot] as EquipmentBackpackData;
            if (equipedBackpack.additionalSlots > additionalSlots && pInfo.playerInventory.Stacks.Count > 12 + additionalSlots)
            {
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", "Inventory Full"), null, 0, NotificationsType.Warning);
                return;
            }
            pInfo.playerInventory.RemoveItem(this, 1);
            
            if (equipedBackpack.additionalSlots < additionalSlots || (equipedBackpack.additionalSlots > additionalSlots) && pInfo.playerInventory.Stacks.Count < 12 + additionalSlots)
            {
                var currentEquipment = eManager.currentEquipment[(int)equipmentSlot];
                eManager.Equip(this, (int)equipmentSlot);
                pInfo.playerInventory.AddItem(currentEquipment, 1, false);
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

}
