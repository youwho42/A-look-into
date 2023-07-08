using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum NotificationType
{
    Inventory,
    Compendium,
    Undertaking,
    Agency,
    Warning,
}

[Serializable]
public class BaseNotificationType
{
    public NotificationType notificationType;
    public Sprite notificationIcon;
    public Color notificationColor;
    public string notificationText;
}
