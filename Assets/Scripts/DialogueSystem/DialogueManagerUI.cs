using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Klaxon.Interactable;


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
        public InteractableDialogue currentInteractable;
        int currentIndex;
        public bool isSpeaking;
        private void Start()
        {
            gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            GameEventManager.onDialogueNextEvent.AddListener(DialogueNextActivate);

        }

        private void OnDisable()
        {
            isSpeaking = false;
            if(currentInteractable!=null)
                currentInteractable.canInteract = true;
            GameEventManager.onDialogueNextEvent.RemoveListener(DialogueNextActivate);
        }



        public void SetNewDialogue(InteractableDialogue interactable, NPC_ConversationSystem convoSystem, DialogueBranch dialogue)
        {
            speakerName.text = convoSystem.NPC_Name.GetLocalizedString();
            currentDialogue = dialogue;
            currentInteractable = interactable;
            currentIndex = 0;
            //messageText.text = currentDialogue.sentences[currentIndex];
            messageText.text = currentDialogue.localizedSentences[currentIndex].GetLocalizedString();
            isSpeaking = true;
        }

        void DialogueNextActivate()
        {
            if (!isSpeaking)
                return;
            currentIndex++;
            if (currentIndex < currentDialogue.localizedSentences.Length)
            {
                
                messageText.text = currentDialogue.localizedSentences[currentIndex].GetLocalizedString();
            }
            else
            {
                isSpeaking = false;
                currentInteractable.canInteract = true;
                UIScreenManager.instance.HideScreenUI();

            }
            
        }
    }
}

