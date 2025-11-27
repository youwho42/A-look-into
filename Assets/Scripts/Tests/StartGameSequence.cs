using UnityEngine;
using QuantumTek.QuantumInventory;
using System.Collections;
using Klaxon.SaveSystem;



public class StartGameSequence : MonoBehaviour
{
    Transform playerTransform;
    public Transform playerStartPosition;
    public Transform playerEndPosition;

    public float playerTransitionDuration;

    public QI_ItemData controlsNotificationItem;

    public AnimationCurve startEase;

    private void Start()
    {
        GameEventManager.onNewGameStartedEvent.AddListener(StartSequence);
        playerTransform = PlayerInformation.instance.player;
        playerTransform.position = playerStartPosition.position;
    }

    private void OnDestroy()
    {
        GameEventManager.onNewGameStartedEvent.RemoveListener(StartSequence);
    }

    public void StartSequence()
    {
        StartCoroutine("StartSequenceCo");
    }


    IEnumerator StartSequenceCo()
    {
        // Move player to start position
        float timer = 0;
        while (timer <= playerTransitionDuration) 
        {
            timer += Time.deltaTime;
            float curve = startEase.Evaluate(timer / playerTransitionDuration);
            var pos = Vector2.Lerp(playerStartPosition.position, playerEndPosition.position, curve);
            playerTransform.position = pos;
            yield return null;
        }


        ShowPlayer();
        yield return new WaitForSeconds(0.5f);

        SetControlsNotification();

        yield return new WaitForSeconds(0.5f);

        SetPlayerActive();
        
        SaveGameStart();
    }

    public void SaveGameStart()
    {
        RealTimeDayNightCycle.instance.isPaused = false;
        SavingLoading.instance.SaveGame();
    }

    public void ShowPlayer()
    {
        LevelManager.instance.NewGamePlayerFadeIn();
    }

    public void SetPlayerActive()
    {
        LevelManager.instance.ActivatePlayer();
    }

    public void SetControlsNotification()
    {
        if (!PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Contains(controlsNotificationItem))
        {
            PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Add(controlsNotificationItem);
            Notifications.instance.SetNewLargeNotification(null, controlsNotificationItem, null, NotificationsType.Compendium);
            GameEventManager.onGuideCompediumUpdateEvent.Invoke();
        }
    }
}
