using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Notification
{
    public QI_ItemData itemData;
    public QI_CraftingRecipe itemRecipe;
    public UndertakingObject undertakingObject;
    public int amount;
    public NotificationType notificationType;
}


public class NotificationCenter : MonoBehaviour
{
    public static NotificationCenter instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    

    public NotificationDisplayObject notificationObject;
    public int maxNotifications;
    public float displayTime;
    //public float fadeTime;
    int currentNotificationCount = 0;

    public Transform notificationActiveHolder;
    public Transform notificationUnactiveHolder;
    public List<BaseNotificationType> notificationTypes = new List<BaseNotificationType>();
    List<NotificationDisplayObject> notificationDisplays = new List<NotificationDisplayObject>();
    Queue<Notification> notificationQueue = new Queue<Notification>();

    private void Start()
    {
        
        for (int i = 0; i < maxNotifications; i++)
        {
            var go = Instantiate(notificationObject);
            go.transform.SetParent(notificationUnactiveHolder.transform, false);
            go.gameObject.SetActive(false);

            notificationDisplays.Add(go);
        }
        
        

    }

    public void SetCompendiumText(QI_ItemData item)
    {
        notificationQueue.Enqueue(new Notification { itemData = item, notificationType = NotificationType.Compendium });
    }

    public void SetCompendiumText(QI_CraftingRecipe recipe)
    {
        notificationQueue.Enqueue(new Notification { itemRecipe = recipe, notificationType = NotificationType.Compendium });
    }

    public void SetUndertakingText(UndertakingObject undertaking)
    {
        notificationQueue.Enqueue(new Notification { undertakingObject = undertaking, notificationType = NotificationType.Undertaking });
    }

    public void SetInventoryText(QI_ItemData item, int amount)
    {
        for (int i = 0; i < notificationDisplays.Count; i++)
        {
            if (notificationDisplays[i].isActiveAndEnabled && notificationDisplays[i].currentNotification.itemData == item)
            {
                //amount += notificationDisplays[i].currentNotification.amount;
                
            }
        }

                //notificationQueue.Enqueue(new Notification { undertakingObject = undertaking, notificationType = NotificationType.Undertaking });
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
        BaseNotificationType baseNotification = null;
        foreach (var type in notificationTypes)
        {
            if(type.notificationType == notification.notificationType)
            {
                baseNotification = type;
            }
        }
        
        int index = 0;
        for (int i = 0; i < notificationDisplays.Count; i++)
        {
            if (!notificationDisplays[i].isActiveAndEnabled)
            {
                
                notificationDisplays[i].SetDisplay(baseNotification, notification);
                notificationDisplays[i].gameObject.transform.SetParent(notificationActiveHolder.transform, false);
                notificationDisplays[i].gameObject.SetActive(true);
                index = i;
                break;
            }
        }


        yield return new WaitForSeconds(displayTime);
        //var display = notificationDisplays[index].GetComponent<CanvasGroup>();
        //float t = 0;
        //while (t < fadeTime)
        //{
        //    t += Time.unscaledDeltaTime;
        //    display.alpha = Mathf.Lerp(1.0f, 0f, t / fadeTime);

        //    yield return null;
        //}

        notificationDisplays[index].gameObject.transform.SetParent(notificationUnactiveHolder.transform, false);
        notificationDisplays[index].gameObject.SetActive(false);
        //display.alpha = 1;
        currentNotificationCount--;

    }
}
