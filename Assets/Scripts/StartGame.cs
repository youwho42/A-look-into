using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using QuantumTek.QuantumDialogue;
using TMPro;

public class StartGame : MonoBehaviour
{

    public CinemachineVirtualCamera startCam;
    public GameObject player;
    public SpriteRenderer shadow;
    public Transform introPosition, startPosition;
    public PlayableDirector director;
    PlayerInputController input;
    Material material;
    

    public QD_DialogueHandler handler;
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI messageText;
    private bool ended;
    public string conversation;
    private void Start()
    {
        input = player.GetComponent<PlayerInputController>();
        material = player.GetComponentInChildren<SpriteRenderer>().material;
        material.SetFloat("_Fade", 0);
        
        player.transform.position = introPosition.position;
        director.Stop();
        handler.SetConversation(conversation);
        SetText();
        ended = true;
    }

    private void Update()
    {
        if (ended)
        {
            HideDialoguePanel();
            return;
        }
        if (handler.currentMessageInfo.Type == QD_NodeType.Message && Input.GetKeyUp(KeyCode.Space))
            Next();
    }
    
    // Start the beginning game timeline and make the player appear
    public void StartBeginningTimeline()
    {
        player.transform.position = startPosition.position;
        director.Play();
    }


    public void BeginGame(bool isNewGame)
    {
        
        if (isNewGame)
            StartCoroutine("BeginNewGameCo");
        else
            StartCoroutine("BeginLoadGameCo");
    }

    IEnumerator BeginNewGameCo()
    {
        
        float timeToWait = 2f;
        
        
        yield return new WaitForSeconds(3f);
        DissolveEffect.instance.StartDissolve(material, timeToWait, true);
        shadow.enabled = true;
        startCam.Priority = 0;
        yield return new WaitForSeconds(4f);
        ended = false;
        DisplayDialoguePanel();
        input.enabled = true;
    }
    IEnumerator BeginLoadGameCo()
    {

        float timeToWait = 2f;

        yield return new WaitForSeconds(3f);
        DissolveEffect.instance.StartDissolve(material, timeToWait, true);
        shadow.enabled = true;
        startCam.Priority = 0;
        yield return new WaitForSeconds(4f);
        
    }

    // The first conversation.
    private void Next(int choice = -1)
    {
        if (ended)
            return;

        // Go to the next message
        handler.NextMessage(choice);
        // Set the new text
        SetText();
        // End if there is no next message
        if (handler.currentMessageInfo.ID < 0)
            ended = true;
    }

    public void SetText()
    {
        // Clear everything
        speakerName.text = "";
        messageText.gameObject.SetActive(false);
        messageText.text = "";
        
        // If at the end, don't do anything
        if (ended)
            return;

        // Generate the display of the message
        if (handler.currentMessageInfo.Type == QD_NodeType.Message)
        {
            QD_Message message = handler.GetMessage();
            speakerName.text = message.SpeakerName;
            messageText.text = message.MessageText;
            messageText.gameObject.SetActive(true);

        }
    }

    public void DisplayDialoguePanel()
    {
        dialoguePanel.SetActive(true);
    }

    public void HideDialoguePanel()
    {
        dialoguePanel.SetActive(false);
        ended = false;
    }
}
