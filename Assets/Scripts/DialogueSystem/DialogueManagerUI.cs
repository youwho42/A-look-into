using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Klaxon.Interactable;
using Klaxon.UndertakingSystem;
using UnityEngine.EventSystems;
using Klaxon.GOAD;


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
        public GameObject NPC_TextObject;
        public GameObject playerResponseObject;

        [HideInInspector]
        public DialogueBranch currentDialogue;
        public GameObject dialoguePanel;
        public InteractableDialogue currentInteractable;
        int currentIndex;
        public bool isSpeaking;

        // New Dialogue System

        DialogueObject currentDialogueObject;
        NPC_DialogueSystem currentDialogueSystem;
        UIScreen screen;
        public List<DialogueResponseObjectUI> dialogueResponseObjects = new List<DialogueResponseObjectUI>();

        bool isInChoice;
        private void Start()
        {
            screen = GetComponent<UIScreen>();
            screen.SetScreenType(UIScreenType.DialogueUI);
       
            gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            //GameEventManager.onDialogueNextEvent.AddListener(DialogueNextActivate);
            GameEventManager.onDialogueNextEvent.AddListener(GoToNextDialogue);

        }

        private void OnDisable()
        {
            isSpeaking = false;
            if(currentInteractable!=null)
                currentInteractable.canInteract = true;
            //GameEventManager.onDialogueNextEvent.RemoveListener(DialogueNextActivate);
            GameEventManager.onDialogueNextEvent.RemoveListener(GoToNextDialogue);
            
        }


        public void StartNewDialogue(InteractableDialogue interactable, NPC_DialogueSystem dialogueSystem, DialogueObject dialogue)
        {
            isSpeaking = true;
            currentDialogueSystem = dialogueSystem;
            currentDialogueObject = dialogue;
            currentInteractable = interactable;
            currentIndex = currentDialogueObject.startPhraseID;
            SetNextDialogue(currentIndex);
        }

        void GoToNextDialogue()
        {
            if (!isSpeaking || isInChoice)
                return;
            if (currentDialogueObject.dialogueNodes[currentIndex].Choices.Count > 0)
            {
                SetChoices();
                return;
            }

            //CompleteDialogueNode();
            
            SetNextDialogue(currentDialogueObject.dialogueNodes[currentIndex].AutoNextNodeID);
        }

        void SetChoices()
        {
            isInChoice = true;

            speakerName.text = PlayerInformation.instance.playerName;
            NPC_TextObject.SetActive(false);
            for (int i = 0; i < dialogueResponseObjects.Count; i++)
            {
                dialogueResponseObjects[i].gameObject.SetActive(false);
                if (i < currentDialogueObject.dialogueNodes[currentIndex].Choices.Count)
                {
                    var choice = currentDialogueObject.dialogueNodes[currentIndex].Choices[i];
                    dialogueResponseObjects[i].gameObject.SetActive(true);
                    dialogueResponseObjects[i].SetResponse(choice.NextNodeID, choice.LocalizedChoice.GetLocalizedString());
                }
            }
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(dialogueResponseObjects[0].gameObject);
            playerResponseObject.SetActive(true);
        }

        public void SetNextDialogue(int nextIndex)
        {
            isInChoice = false;
            NPC_TextObject.SetActive(true);
            playerResponseObject.SetActive(false);
            speakerName.text = currentDialogueSystem.NPC_Name.GetLocalizedString();
            int lastIndex = currentIndex;
            currentIndex = nextIndex;
            if (currentIndex == -1)
            {
                TextToSpeach.instance.StopSpeach();
                currentDialogueObject.hasBeenParsed = true;
                isSpeaking = false;
                currentInteractable.canInteract = true;
                UIScreenManager.instance.HideScreenUI();
                GameEventManager.onUndertakingsUpdateEvent.Invoke();
            }
            else
            {
                messageText.text = currentDialogueObject.dialogueNodes[currentIndex].LocalizedPhrase.GetLocalizedString();
                TextToSpeach.instance.StopSpeach();
                TextToSpeach.instance.ConvertToSpeach(messageText.text, currentDialogueSystem.voicePitch);
                UpdateGumption(lastIndex);
            }
            CompleteDialogueNode(lastIndex);

        }

        void CompleteDialogueNode(int index)
        {
            ActivateUndertaking(index);
            CompleteTask(index);
            SetCondition(index);
        }

        void SetCondition(int index)
        {
            if (!currentDialogueObject.dialogueNodes[index].SetsCondition)
                return;

            if (currentDialogueObject.dialogueNodes[index].IsPersonalBelief)
                currentInteractable.GetComponent<GOAD_Scheduler>().SetBeliefState(currentDialogueObject.dialogueNodes[index].Condition);
            else
                GOAD_WorldBeliefStates.instance.SetWorldState(currentDialogueObject.dialogueNodes[index].Condition);
        }

        void UpdateGumption(int index)
        {
            float factor = currentDialogueObject.dialogueNodes[index].LocalizedPhrase.GetLocalizedString().Length / 100.0f;
            if (currentDialogueSystem.gumptionCost != null)
                PlayerInformation.instance.statHandler.ChangeStat(currentDialogueSystem.gumptionCost, factor);
        }

        void ActivateUndertaking(int index)
        {
            
            if (!currentDialogueObject.dialogueNodes[index].SetsUndertaking)
                return;
            
            currentDialogueObject.dialogueNodes[index].Undertaking.ActivateUndertaking();
        }

        void CompleteTask(int index)
        {
            
            if (!currentDialogueObject.dialogueNodes[index].CompleteTask)
                return;

            
            currentDialogueObject.undertaking.TryCompleteTask(currentDialogueObject.dialogueNodes[index].Task);
            if (currentDialogueObject.dialogueNodes[index].Task.mapName != "")
                    GameEventManager.onMapUpdateEvent.Invoke();
            
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

        //void DialogueNextActivate()
        //{
        //    if (!isSpeaking)
        //        return;
        //    currentIndex++;
        //    if (currentIndex < currentDialogue.localizedSentences.Length)
        //    {
                
        //        messageText.text = currentDialogue.localizedSentences[currentIndex].GetLocalizedString();
        //    }
        //    else
        //    {
                
        //        isSpeaking = false;
        //        currentInteractable.canInteract = true;
        //        UIScreenManager.instance.HideScreenUI();
        //        if (currentDialogue.UndertakingTask != null)
        //        {
        //            if (currentDialogue.UndertakingTask.mapName != "")
        //                GameEventManager.onMapUpdateEvent.Invoke();
        //        }
                

        //    }
            
        //}
    }
}

