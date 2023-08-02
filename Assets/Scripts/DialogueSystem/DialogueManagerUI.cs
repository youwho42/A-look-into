using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Klaxon.ConversationSystem
{
    public class DialogueManagerUI : MonoBehaviour
    {

        public static DialogueManagerUI instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        public TextMeshProUGUI speakerName;
        public TextMeshProUGUI messageText;

        [HideInInspector]
        public DialogueBranch currentDialogue;
        public GameObject dialoguePanel;
        InteractableDialogue currentInteractable;
        int currentIndex;
        bool isSpeaking;

        private void OnEnable()
        {
            GameEventManager.onDialogueNextEvent.AddListener(DialogueNextActivate);

        }

        private void OnDisable()
        {
            GameEventManager.onDialogueNextEvent.RemoveListener(DialogueNextActivate);
        }



        public void SetNewDialogue(InteractableDialogue interactable, NPC_ConversationSystem convoSystem, DialogueBranch dialogue)
        {
            speakerName.text = convoSystem.NPC_Name;
            currentDialogue = dialogue;
            currentInteractable = interactable;
            currentIndex = 0;
            messageText.text = currentDialogue.sentences[currentIndex];
            isSpeaking = true;
        }

        void DialogueNextActivate()
        {
            if (!isSpeaking)
                return;
            currentIndex++;
            if (currentIndex < currentDialogue.sentences.Length)
            {
                
                messageText.text = currentDialogue.sentences[currentIndex];
            }
            else
            {
                isSpeaking = false;
                currentInteractable.canInteract = true;
                UIScreenManager.instance.HideAllScreens();

                if (LevelManager.instance.HUDBinary == 1)
                    UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
                PlayerInformation.instance.uiScreenVisible = false;
                PlayerInformation.instance.TogglePlayerInput(true);

            }
            
        }
    }
}

