using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LostAndFoundManager : MonoBehaviour
{
    public static LostAndFoundManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    public QI_Inventory inventory;


    public void AddToLostAndFound(QI_ItemData item, int amount)
    {
        inventory.AddItem(item, amount, false);
        Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Sent To Lost And Found"), item, amount, NotificationsType.Warning);
    }
}
