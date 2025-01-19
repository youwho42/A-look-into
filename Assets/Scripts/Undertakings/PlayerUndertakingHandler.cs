using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.UndertakingSystem
{
    public class PlayerUndertakingHandler : MonoBehaviour
    {
        public List<UndertakingObject> activeUndertakings = new List<UndertakingObject>();
        Queue<UndertakingObject> addUndertakingQueue = new Queue<UndertakingObject>();
        //Queue<UndertakingTaskObject> completeTaskQueue = new Queue<UndertakingTaskObject>();

        public void AddUndertaking(UndertakingObject undertaking)
        {
            if (!activeUndertakings.Contains(undertaking))
                addUndertakingQueue.Enqueue(undertaking);
                
        }

        public void RestoreUndertaking(UndertakingObject undertaking)
        {
            if (!activeUndertakings.Contains(undertaking))
                activeUndertakings.Add(undertaking);
                
        }
        //public void AddTaskQueue(UndertakingTaskObject task)
        //{
        //    completeTaskQueue.Enqueue(task);
        //}

        private void Update()
        {
            if (addUndertakingQueue.Count > 0 && UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                StartCoroutine(CompleteAddUndertaking(addUndertakingQueue.Dequeue()));
        }

        IEnumerator CompleteAddUndertaking(UndertakingObject undertaking)
        {
            
            activeUndertakings.Add(undertaking);
            undertaking.ActivateUndertaking();
            Notifications.instance.SetNewNotification($"{undertaking.localizedName.GetLocalizedString()}", null, 0, NotificationsType.UndertakingStart);
            yield return new WaitForSeconds(0.3f);
            AudioManager.instance.StartAquiredAudio();
            GameEventManager.onUndertakingsUpdateEvent.Invoke();
            
        }
        //IEnumerator CompleteTaskCo(UndertakingTaskObject undertaking)
        //{

            
        //    //Notifications.instance.SetNewNotification($"{undertaking..localizedName.GetLocalizedString()}", null, 0, NotificationsType.UndertakingStart);
        //    yield return new WaitForSeconds(0.3f);
        //    AudioManager.instance.StartAquiredAudio();
        //    GameEventManager.onUndertakingsUpdateEvent.Invoke();
        //}
    }
}

