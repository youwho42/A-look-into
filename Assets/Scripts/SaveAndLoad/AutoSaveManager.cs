using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class AutoSaveManager : MonoBehaviour
	{
		float nextAutoSaveTime;

        private void Start()
        {
            nextAutoSaveTime = Time.time + 600;
            GameEventManager.onAutoSaveEvent.AddListener(AutoSave);
        }

        private void OnDestroy()
        {
            GameEventManager.onAutoSaveEvent.RemoveListener(AutoSave);
        }

        void AutoSave()
        {
            if (Time.time <= nextAutoSaveTime)
                return;

            nextAutoSaveTime = Time.time + 600;
            SavingLoading.instance.SaveGame();
            Notifications.instance.SetNewNotification("Auto-save", null, 0, NotificationsType.None);
        }

    } 
}
