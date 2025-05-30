using QuantumTek.QuantumInventory;
using System;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.UndertakingSystem;

[Serializable]
public class BaseNotification
{
    public NotificationsType type;

    public QI_ItemData itemData;
    public int quantity;

    public string notificationText;
}
[Serializable]
public class BaseLargeNotification
{
    public NotificationsType type;

    public QI_ItemData itemData;
    public QI_CraftingRecipe itemRecipe;
    public UndertakingObject undertaking;

}


public enum NotificationsType
{
    Compendium,
    Inventory,
    Agency,
    UndertakingStart,
    UndertakingComplete,
    Warning,
    Map,
    None
}

[Serializable]
public struct NotificationTypeColor
{
    public NotificationsType type;
    public Color color;
    public Sprite sprite;
    public Color textColor;
}


public class Notifications : MonoBehaviour
{

    public static Notifications instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public List<NotificationTypeColor> notificationColors = new List<NotificationTypeColor>();

    Queue<BaseNotification> allNotifications = new Queue<BaseNotification>();
    Queue<BaseLargeNotification> allLargeNotifications = new Queue<BaseLargeNotification>();
    List<NotificationDisplayObject> displays = new List<NotificationDisplayObject>();

    public Transform notificationHolder;
    public NotificationLargeDisplayObject largeNotification;
    
    public NotificationDisplayObject notificationDisplayPrefab;
    public int maxNotifications = 5;
    [HideInInspector]
    public int currentNotificationCount = 0;
    [HideInInspector]
    public BaseLargeNotification currentLargeNotificaton;

    private void Start()
    {
        for (int i = 0; i < maxNotifications; i++)
        {
            var go = Instantiate(notificationDisplayPrefab);
            go.transform.SetParent(notificationHolder.transform, false);
            go.gameObject.SetActive(false);

            displays.Add(go);
        }
        largeNotification.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (UIScreenManager.instance.GetIsCurrentUI(UIScreenType.DialogueUI))
            return;

        TryDisplayNotifications();
        TryDisplayLargeNotifications();
    }

    private void TryDisplayNotifications()
    {
        if (currentNotificationCount >= maxNotifications)
            return;
        if (allNotifications.Count > 0)
        {
            ActivateNotification(allNotifications.Dequeue());
            currentNotificationCount++;
        }
    }
    private void TryDisplayLargeNotifications()
    {
       
        if (largeNotification.gameObject.activeInHierarchy)
            return;
        currentLargeNotificaton = null;
        if (allLargeNotifications.Count > 0)
        {
            ActivateLargeNotification(allLargeNotifications.Dequeue());
        }
            
        
    }

    public void SetNewNotification(string message, QI_ItemData item, int amount, NotificationsType notificationType)
    {
        
        var newNotification = new BaseNotification { notificationText = message, itemData = item, quantity = amount, type = notificationType };
        
        if(!UpdateDuplicateDisplays(newNotification))
            allNotifications.Enqueue(newNotification);

    }

    public void SetNewLargeNotification(UndertakingObject newUndertaking, QI_ItemData item, QI_CraftingRecipe recipe, NotificationsType notificationType)
    {
        var newNotification = new BaseLargeNotification { undertaking = newUndertaking, itemData = item, itemRecipe = recipe, type = notificationType };

        allLargeNotifications.Enqueue(newNotification);
    }


    bool UpdateDuplicateDisplays(BaseNotification notification)
    {
        
        for (int i = 0; i < displays.Count; i++)
        {
            if (!displays[i].isActiveAndEnabled)
                continue;

            if (notification.type == NotificationsType.Inventory)
            {
                if (displays[i].notification.itemData == notification.itemData && Mathf.Sign(displays[i].notification.quantity) == Mathf.Sign(notification.quantity))
                {
                    displays[i].UpdateNotification(notification.quantity);
                    return true;
                }

            }
            if(notification.type == NotificationsType.Agency)
            {
                displays[i].UpdateNotification(notification.quantity);
                return true;
            }
            
        }

        return false;
    }

    void ActivateNotification(BaseNotification notification)
    {
        for (int i = 0; i < displays.Count; i++)
        {
            if (displays[i].isActiveAndEnabled)
                continue;

            
            displays[i].notification = notification;
            displays[i].gameObject.SetActive(true);
            displays[i].SetDisplay(notification, SetTypeColor(notification.type));
            
            break;
        }
       
    }
    void ActivateLargeNotification(BaseLargeNotification notification)
    {
        currentLargeNotificaton = notification;
        largeNotification.gameObject.SetActive(true);
        largeNotification.SetDisplay(notification, SetTypeColor(notification.type), allLargeNotifications.Count==0);
    }

    NotificationTypeColor SetTypeColor(NotificationsType type)
    {
        
        NotificationTypeColor col = new NotificationTypeColor();
        for (int i = 0; i < notificationColors.Count; i++)
        {
            if (type == notificationColors[i].type)
                col = notificationColors[i];
        }
        return col;
    }

    public void ClearNotifications()
    {
        currentNotificationCount = 0;
        allNotifications.Clear();
    }
    public void ClearLargeNotifications()
    {
        allLargeNotifications.Clear();
    }
}
