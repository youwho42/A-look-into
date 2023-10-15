using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationDisplayObject : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public Image image;
    public Image notificationImage;

    public Notification currentNotification;
    public BaseNotification notification;
    NotificationTypeColor currentTypeColor;

    public void SetDisplay(string text, Color color, Sprite sprite)
    {
        displayText.text = text;
        image.color = color;
        notificationImage.sprite = sprite;
    }
    public void SetDisplay(BaseNotificationType notificationType, Notification notification)
    {
        currentNotification = notification;
        switch (notificationType.notificationType)
        {
            case NotificationType.Inventory:
                break;

            case NotificationType.Compendium:
                SetCompendiumDisplay(notificationType, notification);
                break;

            case NotificationType.Undertaking:
                SetUndertakingDisplay(notificationType, notification);
                break;

            case NotificationType.Agency:
                break;

            case NotificationType.Warning:
                break;

        }
       
    }

    public void SetDisplay(BaseNotification note, NotificationTypeColor typeColor)
    {
        notification = note;
        currentTypeColor = typeColor;
        string text = "";
        switch (notification.type)
        {
            
            case NotificationsType.Compendium:
                break;
            case NotificationsType.Inventory:
                text = $"{notification.itemData.localizedName.GetLocalizedString()} {note.quantity}";
                UpdateDisplay(currentTypeColor, text);
                break;
            case NotificationsType.Warning:
                break;
            case NotificationsType.Undertaking:
                break;
            case NotificationsType.Agency:
                break;
            case NotificationsType.None:
                break;
            
        }

    }

    public void SetCompendiumDisplay(BaseNotificationType notificationType, Notification notification)
    {
        string item = "";
        if (notification.itemData != null)
            item = notification.itemData.Name;
        else if (notification.itemRecipe != null)
            item = $"{notification.itemRecipe.Name} recipe";

        UpdateDisplay(notificationType, $"{item} added");
        
    }
    public void SetUndertakingDisplay(BaseNotificationType notificationType, Notification notification)
    {
        string state = notification.undertakingObject.CurrentState.ToString();

        UpdateDisplay(notificationType, $"{notification.undertakingObject.Name} Undertaking {notification.undertakingObject.CurrentState}");

        
    }

    public void SetInventoryDisplay(BaseNotificationType notificationType, Notification notification)
    {

    }

    public void UpdateNotification(int additionalAmount)
    {
        notification.quantity += additionalAmount;
        SetDisplay(notification, currentTypeColor);
    }

    void UpdateDisplay(BaseNotificationType notificationType, string notificationText)
    {
        image.color = notificationType.notificationColor;
        notificationImage.sprite = notificationType.notificationIcon;
        displayText.text = notificationText;
    }

    void UpdateDisplay(NotificationTypeColor notificationType, string notificationText)
    {
        image.color = notificationType.color;
        notificationImage.sprite = notificationType.sprite;
        displayText.color= notificationType.textColor;
        displayText.text = notificationText;
    }

    
}
