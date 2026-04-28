using UnityEngine;
using System.Collections.Generic;
using Klaxon.GOAD;
using Klaxon.Interactable;
using System.Collections;
using Klaxon.ConversationSystem;
using Klaxon.SaveSystem;

public class PokableBear : PokableItem
{

    GOAD_Scheduler_Animal animal;
    InteractableDialogue interactableDialogue;
    NPC_DialogueSystem npcDialogueSystem;
    PlayerInformation player;

    public GOAD_ScriptableCondition BearMetPlayer;
    public GOAD_ScriptableCondition BearConsequenceOne;
    public GOAD_ScriptableCondition BearConsequenceTwo;
    public GOAD_ScriptableCondition BearConsequenceThree;

    bool isPoked;
    private void Start()
    {
        isPoked = false;
        animal = GetComponent<GOAD_Scheduler_Animal>();
        interactableDialogue = GetComponent<InteractableDialogue>();
        npcDialogueSystem = GetComponent<NPC_DialogueSystem>();
        player = PlayerInformation.instance;
    }

    private void OnEnable()
    {
        GameEventManager.onPokableMinigameStartEvent.AddListener(IsPoking);
        
        
    }

    private void OnDisable()
    {
        GameEventManager.onPokableMinigameStartEvent.RemoveListener(IsPoking);
        
    }

    void IsPoking(PokableItem pokable)
    {
        if (pokable == this)
        {
            isPoked = true;
            GameEventManager.onDialogueEndEvent.AddListener(EnactConsequence);
            animal.SetTalkAnimation();
        }
            
    }
    void EnactConsequence()
    {
        StartCoroutine(EnactConsequenceCo());
    }
    IEnumerator EnactConsequenceCo()
    {
        yield return new WaitForSeconds(0.5f);
        animal.CancelTalkAnimation();
        if (isPoked)
        {
            int consequence = GetCurrentConsequenceInt();

            if (consequence == 4)
            {
                //Debug.Log("Last Consequence");
                StartCoroutine(DestroyPlayerSave());
                EndEvent();
                
            }
            else if (consequence == 3)
            {
                //Debug.Log("Second Consequence");
                StartCoroutine(RemoveAllPlayerInventory(true));


                EndEvent();

            }
            else if (consequence == 2)
            {
                //Debug.Log("First Consequence");
                StartCoroutine(RemoveAllPlayerInventory(false));
                

                EndEvent();

            }
            else if (consequence == 1)
            {
                //Debug.Log("Warning");
                EndEvent();
                
            }
            

        }
    }
    IEnumerator DestroyPlayerSave()
    {
        AudioManager.instance.PlaySound("BearFail");
        SavingLoading.instance.DeleteFile($"{Application.persistentDataPath}/{player.playerName}_save.ali");
        yield return new WaitForSeconds(1.0f);
        player.runningManager.Land(player.playerController.currentLevel + 20);
        ScreenFadeManager.instance.FadeScreen(1.9f, false);
        yield return new WaitForSeconds(1.905f);

        LevelManager.instance.LoadTitleScreen();
        yield return null;
    }

    IEnumerator RemoveAllPlayerInventory(bool destroyItems)
    {
        foreach (var item in player.playerInventory.Stacks)
        {
            var go = Instantiate(item.Item.ItemPrefabVariants[0], player.playerTransform.position, Quaternion.identity);
            if (go.TryGetComponent(out InteractablePickUp interactable))
                interactable.pickupQuantity = item.Amount;
            //if(destroyItems)
            //    interactable.canInteract = false;

            if (go.TryGetComponent(out SaveableItemEntity saveable))
                saveable.GenerateId();

            player.runningManager.Land(player.playerController.currentLevel + 2);
            AudioManager.instance.PlaySoundWithDelay("ScanFail", 0.1f);
            Vector2 offset = Random.insideUnitCircle * Random.Range(0.1f, 0.35f);
            Vector3 pos = player.playerTransform.position + (Vector3)offset;
            float maxT = 0.3f;
            float t = 0.0f;
            while (t < maxT)
            {
                t += Time.deltaTime;
                var p = Vector3.Lerp(player.playerTransform.position, pos, t / maxT);
                go.transform.position = p;
                yield return null;
            }
            if (destroyItems)
            {
                //yield return new WaitForSeconds(0.01f);
                Destroy(go.gameObject);
            }
            yield return null;
        }
        player.playerInventory.RemoveAllItems();

        

        if(player.equipmentManager.HasAnyItemEquipped())
        {
            player.equipmentManager.UnEquipAllToInventory();
            StartCoroutine(RemoveAllPlayerInventory(destroyItems));
        }
        yield return null;
    }

    int GetCurrentConsequenceInt()
    {
        int total = 0;
        if (animal.HasBelief(BearMetPlayer.Condition, BearMetPlayer.State))
            total++;
        if (animal.HasBelief(BearConsequenceOne.Condition, BearConsequenceOne.State))
            total++;
        if (animal.HasBelief(BearConsequenceTwo.Condition, BearConsequenceTwo.State))
            total++;
        if (animal.HasBelief(BearConsequenceThree.Condition, BearConsequenceThree.State))
            total++;
        return total;
    }

    void EndEvent()
    {
        isPoked = false;
        interactableDialogue.ResetInteraction();
        npcDialogueSystem.hasRandomTalked = false;
        GameEventManager.onDialogueEndEvent.RemoveListener(EnactConsequence);
    }
    public override void PokeItemSuccess()
    {
        base.PokeItemSuccess();
        animal.SetTalkAnimation();
        StartCoroutine(DialogueStartDelayCo());
    }

    public override void PokeItemFail()
    {
        base.PokeItemFail();
        isPoked = false;
        EndEvent();
    }


    IEnumerator DialogueStartDelayCo()
    {
        npcDialogueSystem.hasRandomTalked = false;
        animal.SetTalkAnimation();
        yield return new WaitForSeconds(1.5f);
        interactableDialogue.Interact(PlayerInformation.instance.playerTransform.gameObject);
    }
}
