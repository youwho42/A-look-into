using QuantumTek.QuantumDialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class IntroCutsceneManager : MonoBehaviour
{

    PlayableDirector introCutsceneDirector;
    public PlayableDirector chickenDirector;
    public TextMeshProUGUI speaker;
    public TextMeshProUGUI message;
    public QD_DialogueHandler handler;
    bool ended;
    //bool canSkip;
    private void Start()
    {
        GameEventManager.onNewGameStartedEvent.AddListener(PlayCutscene);
        GameEventManager.onGameLoadedEvent.AddListener(SetChicken);
        introCutsceneDirector = GetComponent<PlayableDirector>();
        handler.SetConversation("ChickenIntroduction");
        SetText();
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
        
        PlayerInformation.instance.playerQuestHandler.ActivateQuest("Talk to Manuela or Ramiro");
        

        GameEventManager.onUndertakingsUpdateEvent.Invoke();
    }

    void SetText()
    {
        // Clear everything
        speaker.text = "";
        message.text = "";

        
        // Generate choices if a choice, otherwise display the message
        if (handler.currentMessageInfo.Type == QD_NodeType.Message)
        {
            QD_Message messageToDisplay = handler.GetMessage();
            speaker.text = messageToDisplay.SpeakerName;
            message.text = messageToDisplay.MessageText;
        }
        
    }

    public void Next()
    {
        if (ended)
            return;

        // Go to the next message
        handler.NextMessage(-1);
        // Set the new text
        SetText();
        // End if there is no next message
        if (handler.currentMessageInfo.ID < 0)
            ended = true;
    }
}
