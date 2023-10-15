using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[Serializable]
public class BaseNotification
{
    public NotificationsType type;

    public QI_ItemData itemData;
    public int quantity;

    public string notificationText;
}


public enum NotificationsType
{
    Compendium,
    Inventory,
    Warning,
    Undertaking,
    Agency,
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
    List<NotificationDisplayObject> displays = new List<NotificationDisplayObject>();

    public Transform notificationHolder;
    
    public NotificationDisplayObject notificationDisplayPrefab;
    int maxNotifications = 5;
    int currentNotificationCount = 0;
    

    private void Start()
    {
        for (int i = 0; i < maxNotifications; i++)
        {
            var go = Instantiate(notificationDisplayPrefab);
            go.transform.SetParent(notificationHolder.transform, false);
            go.gameObject.SetActive(false);

            displays.Add(go);
        }
    }

    private void Update()
    {
        if (currentNotificationCount >= maxNotifications)
            return;
        if (allNotifications.Count > 0)
        {
            ActivateNotification(allNotifications.Dequeue());
            currentNotificationCount++;
        }
    }



    public void SetNewNotification(string message, QI_ItemData item, int amount, NotificationsType notificationType)
    {
        
        var newNotification = new BaseNotification { notificationText = message, itemData = item, quantity = amount, type = notificationType };
        
        if(!UpdateDuplicateDisplays(newNotification))
            allNotifications.Enqueue(newNotification);

    }


    bool UpdateDuplicateDisplays(BaseNotification notification)
    {
        
        for (int i = 0; i < displays.Count; i++)
        {
            if (!displays[i].isActiveAndEnabled)
                continue;

            if (notification.type == NotificationsType.Inventory)
            {
                if (displays[i].notification.itemData == notification.itemData)
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
            displays[i].SetDisplay(notification, SetTypeColor(notification.type));
            displays[i].gameObject.SetActive(true);
            break;
        }
       
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
}
