using Klaxon.GOAD;
using Klaxon.Interactable;
using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class FixAndDeactivate : MonoBehaviour, IFixArea
{

    public ParticleSystem fixingEffect;
    public SpriteRenderer fixableSprite;
    public GameObject fixableReplacementObject;
    bool isFixing;

    public bool needsActiveUndertaking;
    
    public CompleteTaskObject undertakingObject;

    public GOAD_ScriptableCondition GOAD_Condition;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        if (GOAD_WorldBeliefStates.instance.HasState(GOAD_Condition.Condition, GOAD_Condition.State))
        {
            fixableReplacementObject.SetActive(true);
            gameObject.SetActive(false);
        }

    }

    public bool Fix(List<FixableAreaIngredient> ingredients)
    {
        if(needsActiveUndertaking)
        {
            if (undertakingObject.undertaking.CurrentState != UndertakingState.Active)
            {
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Unavailable"), null, 0, NotificationsType.Warning);
                return false;
            }
                
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
        fixableReplacementObject.SetActive(true);

        yield return new WaitForSeconds(3);


        GOAD_WorldBeliefStates.instance.SetWorldState(GOAD_Condition.Condition, GOAD_Condition.State);
        if (undertakingObject.undertaking != null)
            undertakingObject.undertaking.TryCompleteTask(undertakingObject.task);
        player.playerInput.isInUI = false;
        player.animatePlayerScript.SetCraftAnimation(false);
        
        gameObject.SetActive(false);
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
