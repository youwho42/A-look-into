using Klaxon.SaveSystem;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;

public class FixAndReplace : MonoBehaviour, IFixArea
{
    public ParticleSystem fixingEffect;
    public GameObject fixableReplacementObject;
    public SpriteRenderer fixableSprite;
    bool isFixing;

    public CompleteTaskObject undertakingObject;
    public bool replacementObjectHasLongInteract;
    public bool needsActiveUndertaking;
    [ConditionalHide("needsActiveUndertaking", true)]
    public UndertakingObject neededUndertaking;

    public bool Fix(List<FixableAreaIngredient> ingredients)
    {
        if (needsActiveUndertaking)
        {
            if(neededUndertaking.CurrentState == UndertakingState.Inactive)
                neededUndertaking.ActivateUndertaking();
               
        }

        if (!isFixing)
            StartCoroutine(FixCo(ingredients));
        return true;
    }

    IEnumerator FixCo(List<FixableAreaIngredient> ingredients)
    {
        var player = PlayerInformation.instance;
        player.playerInput.isInUI = true;
        player.animatePlayerScript.SetCraftAnimation(true);
        isFixing = true;
        RemoveItemsFromInventory(ingredients);
        fixingEffect.Play();
        float timer = 0;
        float timeToFade = 4;
        while (timer < timeToFade)
        {
            float a = Mathf.Lerp(1, 0, timer / timeToFade);
            fixableSprite.color = new Color(fixableSprite.color.r, fixableSprite.color.r, fixableSprite.color.r, a);
            timer += Time.deltaTime;
            yield return null;
        }
        var go = Instantiate(fixableReplacementObject, transform.position, Quaternion.identity);
        if(go.TryGetComponent(out SaveableItemEntity item))
            item.GenerateId();

        if (go.TryGetComponent(out ActivateOnQuestComplete obj))
            obj.undertakingName = undertakingObject.undertaking.Name;

        if (go.TryGetComponent(out Interactable interactable))
            interactable.hasLongInteract = replacementObjectHasLongInteract;

        fixableSprite.color = new Color(fixableSprite.color.r, fixableSprite.color.r, fixableSprite.color.r, 0);
        yield return new WaitForSeconds(3);
        if(fixableReplacementObject.TryGetComponent(out QI_Item data))
        {
            if (data.Data.compendiumGuide != null)
            {
                if (!player.playerGuidesCompendiumDatabase.Items.Contains(data.Data.compendiumGuide))
                {
                    player.playerGuidesCompendiumDatabase.Items.Add(data.Data.compendiumGuide);
                    Notifications.instance.SetNewNotification($"{data.Data.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Compendium);

                    //NotificationManager.instance.SetNewNotification($"{data.Data.compendiumGuide.Name} added to guides", NotificationManager.NotificationType.Compendium);
                    GameEventManager.onGuideCompediumUpdateEvent.Invoke();
                }
            }
        }
        if (undertakingObject.undertaking != null)
            undertakingObject.undertaking.TryCompleteTask(undertakingObject.task);
        player.playerInput.isInUI = false;
        player.animatePlayerScript.SetCraftAnimation(false);
        Destroy(this.gameObject);
    }
    void RemoveItemsFromInventory(List<FixableAreaIngredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            PlayerInformation.instance.playerInventory.RemoveItem(ingredient.item.Name, ingredient.amount);
            Notifications.instance.SetNewNotification("", ingredient.item, -ingredient.amount, NotificationsType.Inventory);

        }
    }
}
