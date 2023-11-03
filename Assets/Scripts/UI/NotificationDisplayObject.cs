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

    //public Notification currentNotification;
    public BaseNotification notification;
    NotificationTypeColor currentTypeColor;
    float maxDisplayTime = 3f;
    float maxFadeTime = 0.5f;

    CanvasGroup displayGroup;

 

    

    public void SetDisplay(BaseNotification note, NotificationTypeColor typeColor)
    {
        displayGroup = GetComponent<CanvasGroup>();
        notification = note;
        currentTypeColor = typeColor;
        string text = "";
        switch (notification.type)
        {
            
            case NotificationsType.Compendium:
                text = $"{notification.notificationText}";
                UpdateDisplay(currentTypeColor, text);
                break;
            case NotificationsType.Inventory:
                text = $"{notification.itemData.localizedName.GetLocalizedString()} {note.quantity}";
                UpdateDisplay(currentTypeColor, text);
                break;
            case NotificationsType.Agency:
                text = $"<sprite name=\"Agency\"> {note.quantity}";
                UpdateDisplay(currentTypeColor, text);
                break;
            case NotificationsType.UndertakingStart:
                text = $"{notification.notificationText}";
                UpdateDisplay(currentTypeColor, text);
                break;
            case NotificationsType.UndertakingComplete:
                text = $"{notification.notificationText}";
                UpdateDisplay(currentTypeColor, text);
                break;
            case NotificationsType.Warning:
                text = $"{notification.notificationText}";
                UpdateDisplay(currentTypeColor, text);
                break;
            case NotificationsType.None:
                text = $"{notification.notificationText}";
                UpdateDisplay(currentTypeColor, text);
                break;
            
        }

    }

    

    public void UpdateNotification(int additionalAmount)
    {
        notification.quantity += additionalAmount;
        SetDisplay(notification, currentTypeColor);
    }

    
    void UpdateDisplay(NotificationTypeColor notificationType, string notificationText)
    {
        displayGroup.alpha = 1;
        image.color = notificationType.color;
        notificationImage.sprite = notificationType.sprite;
        displayText.color= notificationType.textColor;
        displayText.text = notificationText;
        StopCoroutine("DisplayCo");
        StartCoroutine("DisplayCo");
    }

    IEnumerator DisplayCo()
    {
        //var obj = image.GetComponent<RectTransform>();
        //StartCoroutine(ShakeIcon(obj, obj.anchoredPosition, 0.5f));
        float dt = 0;
        while (dt < maxDisplayTime)
        {
            dt += Time.deltaTime;
            yield return null;

        }
        float t = 0;
        while (t < maxFadeTime)
        {
            t += Time.deltaTime;
            displayGroup.alpha = Mathf.Lerp(1.0f, 0f, t / maxFadeTime);
            yield return null;
        }
        gameObject.SetActive(false);
        Notifications.instance.currentNotificationCount--;
    }

    IEnumerator ShakeIcon(RectTransform statObject, Vector2 originalPos, float diff)
    {

        float shakeAmount = 0.7f;
        //float shakeTime = 0.1f;
        float decreaseFactor = 1.0f;
        float shakeDistance = 4f;


        float currentShakeAmount = shakeAmount;

        var shakeDuration = diff;
        while (shakeDuration > 0)
        {
            statObject.anchoredPosition = originalPos + new Vector2(Random.Range(-shakeDistance, shakeDistance), Random.Range(-shakeDistance, shakeDistance)) * currentShakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
            currentShakeAmount = Mathf.Lerp(currentShakeAmount, 0, Time.deltaTime * decreaseFactor);

            yield return null;
        }

        statObject.anchoredPosition = originalPos;
    }

}
