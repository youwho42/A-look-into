using Klaxon.ConversationSystem;
using Klaxon.GOAD;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.Interactable
{
    public class InteractableDialogue : Interactable
    {

        //NPC_ConversationSystem dialogueSystem;
        DialogueBranch currentDialogue;
        public GOAD_ScriptableCondition hasMetPlayerCondition;
        GOAD_Scheduler_NPC agentNPC;
        GOAD_Scheduler_Ghost agentGhost;
        QI_Item characterItem;

        NPC_DialogueSystem npc_DialogueSystem;
        DialogueObject currentDialogueObject;


        public override void Start()
        {
            base.Start();
            characterItem = GetComponent<QI_Item>();
            agentNPC = GetComponent<GOAD_Scheduler_NPC>();
            agentGhost = GetComponent<GOAD_Scheduler_Ghost>();
            //dialogueSystem = GetComponent<NPC_ConversationSystem>();
            npc_DialogueSystem = GetComponent<NPC_DialogueSystem>();
        }
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (!HasEnoughGumption())
                return;
            if(npc_DialogueSystem != null)
            {
                if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                {
                    currentDialogueObject = npc_DialogueSystem.GetDialogue();
                    if (currentDialogueObject == null)
                        return;
                    
                    if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.DialogueUI, true))
                    {
                        
                        if (characterItem != null)
                            PlayerInformation.instance.playerEncountersCompendiumDatabase.AddItem(characterItem.Data);

                        if (TryGetComponent(out GOAD_Scheduler_NPC npc))
                            npc.StopNPC(interactor.transform.position);
                        canInteract = false;

                        DialogueManagerUI.instance.StartNewDialogue(this, npc_DialogueSystem, currentDialogueObject);

                        PlayerInformation.instance.uiScreenVisible = true;
                        PlayerInformation.instance.TogglePlayerInput(false);

                    }
                }
            }
            //else
            //{
            //    if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            //    {
            //        currentDialogue = dialogueSystem.GetConversation();
            //        if (currentDialogue == null)
            //            return;

            //        if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.DialogueUI, true))
            //        {
            //            //if (hasMetPlayerCondition != null)
            //            //{
            //            if (characterItem != null)
            //                PlayerInformation.instance.playerEncountersCompendiumDatabase.AddItem(characterItem.Data);

            //            //    if (agentNPC != null)
            //            //        agentNPC.SetBeliefState(hasMetPlayerCondition.Condition, true);
            //            //    if (agentGhost != null)
            //            //        agentGhost.SetBeliefState(hasMetPlayerCondition.Condition, true);
            //            //}


            //            if (TryGetComponent(out GOAD_Scheduler_NPC npc))
            //                npc.StopNPC(interactor.transform.position);
            //            canInteract = false;
            //            DialogueManagerUI.instance.SetNewDialogue(this, dialogueSystem, currentDialogue);

            //            PlayerInformation.instance.uiScreenVisible = true;
            //            PlayerInformation.instance.TogglePlayerInput(false);
            //        }

            //    }
            //}

            
        }
        bool HasEnoughGumption()
        {

            if (npc_DialogueSystem.gumptionCost == null)
                return true;


            float gumption = PlayerInformation.instance.statHandler.GetStatCurrentModifiedValue(npc_DialogueSystem.gumptionCost.StatToModify);
            if (gumption >= Mathf.Abs(npc_DialogueSystem.gumptionCost.Amount))
                return true;
            
            Notifications.instance.SetNewNotification($"{Mathf.CeilToInt(Mathf.Abs(npc_DialogueSystem.gumptionCost.Amount) - gumption)} <sprite name=\"Gumption\">", null, 0, NotificationsType.Warning);
            return false;
        }

    } 
}

