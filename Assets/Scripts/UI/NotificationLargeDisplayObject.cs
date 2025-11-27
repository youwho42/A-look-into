using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationLargeDisplayObject : MonoBehaviour
{
    public TextMeshProUGUI displayTitle;
    public TextMeshProUGUI displayDescription;
    public Image notificationIcon;
    public Image notificationBackground;

    //public Notification currentNotification;
    [HideInInspector]
    public BaseLargeNotification notification;
    NotificationTypeColor currentTypeColor;
    float maxDisplayTime = 5f;
    float maxFadeTime = 0.5f;
    public Image largeNotifSliderBG;
    public Image largeNotifSliderFill;
    CanvasGroup displayGroup;
    public Slider interactSlider;

    public void SetDisplay(BaseLargeNotification note, NotificationTypeColor typeColor, bool last)
    {
        displayGroup = GetComponent<CanvasGroup>();
        notification = note;
        currentTypeColor = typeColor;
        string title = "";
        string description = "";
        
        maxDisplayTime = last ? 5f:2.5f;
        maxFadeTime = last? 0.5f:0.2f;
        
        switch (notification.type)
        {
            case NotificationsType.Compendium:
                title = $"Knowledge Gained!";
                description = $"<style=\"Bold\">{notification.itemData.localizedName.GetLocalizedString()}</style> knowledge added to your compendium";
                UpdateDisplay(currentTypeColor, title, description);
                break;
            case NotificationsType.UndertakingStart:
                if(notification.undertaking.CurrentState != Klaxon.UndertakingSystem.UndertakingState.Complete)
                {
                    title = $"{notification.undertaking.localizedName.GetLocalizedString()}";
                    description = $"{notification.undertaking.localizedDescription.GetLocalizedString()}";
                    UpdateDisplay(currentTypeColor, title, description);
                }
                break;
            case NotificationsType.UndertakingComplete:
                title = $"{notification.undertaking.localizedName.GetLocalizedString()}";
                description = $"{notification.undertaking.localizedCompleteDescription.GetLocalizedString()}";
                UpdateDisplay(currentTypeColor, title, description);
                break;
        }

    }

    void UpdateDisplay(NotificationTypeColor notificationType, string notificationTitle, string notificationDescription)
    {
        displayGroup.alpha = 1;
        interactSlider.value = 0;
        notificationBackground.color = notificationType.color;
        notificationIcon.sprite = notificationType.sprite;
        displayTitle.color = notificationType.textColor;
        displayDescription.color = notificationType.textColor;
        largeNotifSliderBG.color = notificationType.color*1.1f;
        largeNotifSliderFill.color = notificationType.color*.7f;
        displayTitle.text = notificationTitle;
        displayDescription.text = notificationDescription;
        //StopCoroutine("DisplayCo");
        StartCoroutine("DisplayCo");
    }

    IEnumerator DisplayCo()
    {
        yield return new WaitForSeconds(maxDisplayTime);
        //float dt = 0;
        //while (dt < maxDisplayTime)
        //{
        //    dt += Time.deltaTime;
        //    yield return null;

        //}
        float t = 0;
        while (t < maxFadeTime)
        {
            t += Time.deltaTime;
            displayGroup.alpha = Mathf.Lerp(1.0f, 0f, t / maxFadeTime);
            yield return null;
        }
        gameObject.SetActive(false);
        yield return null;
    }


}
