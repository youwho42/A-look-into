using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;
    public struct Notification
    {
        public string textToDisplay;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public GameObject notificationDisplayPrefab;

    public TextMeshProUGUI notificationText;
    public float displayTime;
    public float fadeTime;
    Queue<Notification> notificationQueue = new Queue<Notification>();

    private Coroutine notificationCoroutine;
    bool notificationActive;

    public void SetNewNotification(string message)
    {
        
        notificationQueue.Enqueue(new Notification { textToDisplay = message });

    }
    private void Update()
    {

        if (notificationQueue.Count > 0 && !notificationActive)
        {
            if (notificationCoroutine != null)
                StopCoroutine(notificationCoroutine);
            notificationCoroutine = StartCoroutine(FadeOutNotificationCo(notificationQueue.Dequeue().textToDisplay));
        }
    }

    IEnumerator FadeOutNotificationCo(string message)
    {
        notificationActive = true;
        notificationText.text = message;
        notificationText.color = new Color(
                notificationText.color.r,
                notificationText.color.g,
                notificationText.color.b,
                1f);
        yield return new WaitForSeconds(displayTime);
        notificationActive = false;
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            notificationText.color = new Color(
                notificationText.color.r, 
                notificationText.color.g, 
                notificationText.color.b, 
                Mathf.Lerp(1.0f, 0f, t / fadeTime));

            yield return null;
        }
        notificationCoroutine = null;
    }
    
}
