
using Klaxon.SaveSystem;
using QuantumTek.QuantumInventory;
using Klaxon.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class IntroCutsceneManager : MonoBehaviour
{

    PlayableDirector introCutsceneDirector;
    public PlayableDirector chickenDirector;
    public TextMeshProUGUI speaker;
    public TextMeshProUGUI message;
    //public QI_ItemData messageItem;

    public QI_ItemData messageControls;
    bool ended;
    //bool canSkip;
    private void Start()
    {
        GameEventManager.onNewGameStartedEvent.AddListener(PlayCutscene);
        GameEventManager.onGameLoadedEvent.AddListener(SetChicken);
        introCutsceneDirector = GetComponent<PlayableDirector>();
        
    }
    private void OnDisable()
    {
        GameEventManager.onNewGameStartedEvent.RemoveListener(PlayCutscene);
        GameEventManager.onGameLoadedEvent.RemoveListener(SetChicken);
    }
    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Space) && canSkip)
    //    {
    //        introCutsceneDirector.time = introCutsceneDirector.duration - 0.1f;
    //        message.text = "";
    //    }
    //}
    public void PlayCutscene()
    {
        introCutsceneDirector.Play();
    }

    public void LoadPlayer()
    {
        LevelManager.instance.NewGamePlayerFadeIn();
    }

    public void SaveGameStart()
    {
        RealTimeDayNightCycle.instance.isPaused = false;
        SavingLoading.instance.Save();
    }

    public void SetPlayerActive()
    {
        LevelManager.instance.ActivatePlayer();
    }
    void SetChicken()
    {
        chickenDirector.Play();
    }
    
    //public void SetCanSkip()
    //{
    //    canSkip = true;
    //}

    public void SetFirstQuest()
    {

        //PlayerInformation.instance.playerQuestHandler.ActivateQuest("Talk to Manuela or Ramiro");
        //PlayerInformation.instance.playerUndertakings.AddUndertaking(startUndertaking);

        //GameEventManager.onUndertakingsUpdateEvent.Invoke();
        //var pos = PlayerInformation.instance.player.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.0f),0);
        //BallPeopleManager.instance.SpawnMessenger(messageControls, BallPeopleMessageType.Guide, null, null, pos);
        if (!PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Contains(messageControls))
        {
            PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Add(messageControls);
            Notifications.instance.SetNewNotification(messageControls.localizedName.GetLocalizedString(), messageControls, 1, NotificationsType.Compendium);
            GameEventManager.onGuideCompediumUpdateEvent.Invoke();
        }
    }

    

    
}
