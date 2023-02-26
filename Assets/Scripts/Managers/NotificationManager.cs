using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;
    public struct Notification
    {
        public string textToDisplay;
        public NotificationType type;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    [Serializable]
    public enum NotificationType
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
        public NotificationType notificationType;
        public Color color;
        public Sprite sprite;
    }

    public List<NotificationTypeColor> notificationTypes = new List<NotificationTypeColor>();

    public Transform notificationActiveHolder;
    public Transform notificationUnactiveHolder;
    public NotificationDisplayObject notificationDisplayPrefab;
    int maxNotifications = 5;
    int currentNotificationCount = 0;
    List<NotificationDisplayObject> notificationDisplays = new List<NotificationDisplayObject>();

    public TextMeshProUGUI notificationText;
    public float displayTime;
    public float fadeTime;
    Queue<Notification> notificationQueue = new Queue<Notification>();

    private void Start()
    {
        for (int i = 0; i < maxNotifications; i++)
        {
            var go = Instantiate(notificationDisplayPrefab);
            go.transform.SetParent(notificationUnactiveHolder.transform, false);
            go.gameObject.SetActive(false);

            notificationDisplays.Add(go);
        }
    }

    public void SetNewNotification(string message, NotificationType notificationType = NotificationType.None)
    {
        notificationQueue.Enqueue(new Notification {textToDisplay = message, type = notificationType});
    }

    private void Update()
    {
        if (currentNotificationCount >= maxNotifications)
            return;
        if (notificationQueue.Count > 0)
        {
            StartCoroutine(DisplayNotificationCo(notificationQueue.Dequeue()));
            currentNotificationCount++;
        }
    }

    IEnumerator DisplayNotificationCo(Notification notification)
    {
        Color color = SetNotificationColor(notification);
        Sprite sprite = SetNotificationSprite(notification);
        int index = 0;
        //for (int i = 0; i < notificationDisplays.Count; i++)
        //{
        //    if (notificationDisplays[i].isActiveAndEnabled && notificationDisplays[i].displayText.text == notification.textToDisplay)
        //    {
        //        Debug.Log("more than one similar notification");
        //    }
        //}
        for (int i = 0; i < notificationDisplays.Count; i++)
        {
            if (!notificationDisplays[i].isActiveAndEnabled)
            {
                index = i;
                notificationDisplays[i].SetDisplay(notification.textToDisplay, color, sprite);
                notificationDisplays[i].gameObject.transform.SetParent(notificationActiveHolder.transform, false);
                notificationDisplays[i].gameObject.SetActive(true);
                break;
            }
        }
        

        yield return new WaitForSeconds(displayTime);
        var display = notificationDisplays[index].GetComponent<CanvasGroup>();
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            display.alpha = Mathf.Lerp(1.0f, 0f, t / fadeTime);
            
            yield return null;
        }

        notificationDisplays[index].gameObject.transform.SetParent(notificationUnactiveHolder.transform, false);
        notificationDisplays[index].gameObject.SetActive(false);
        display.alpha = 1;
        currentNotificationCount--;
        
    }
    Color SetNotificationColor(Notification notification)
    {
        Color color = Color.white;
        for (int i = 0; i < notificationTypes.Count; i++)
        {
            if (notification.type == notificationTypes[i].notificationType)
            {
                color = notificationTypes[i].color;
                return color;
            }
        }
        return Color.white;
    }

    Sprite SetNotificationSprite(Notification notification)
    {
        Sprite sprite = null;
        for (int i = 0; i < notificationTypes.Count; i++)
        {
            if (notification.type == notificationTypes[i].notificationType)
            {
                
                sprite = notificationTypes[i].sprite;
                return sprite;
                
            }
        }
        return sprite;
    }


    //IEnumerator FadeOutNotificationCo(string message)
    //{
    //    notificationActive = true;
    //    notificationText.text = message;
    //    notificationText.color = new Color(
    //            notificationText.color.r,
    //            notificationText.color.g,
    //            notificationText.color.b,
    //            1f);
    //    yield return new WaitForSeconds(displayTime);
    //    notificationActive = false;
    //    float t = 0;
    //    while (t < fadeTime)
    //    {
    //        t += Time.unscaledDeltaTime;
    //        notificationText.color = new Color(
    //            notificationText.color.r, 
    //            notificationText.color.g, 
    //            notificationText.color.b, 
    //            Mathf.Lerp(1.0f, 0f, t / fadeTime));

    //        yield return null;
    //    }
    //    notificationCoroutine = null;
    //}

}
