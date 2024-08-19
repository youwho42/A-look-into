using Klaxon.GOAD;
using Klaxon.Interactable;
using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAndDeactivate : MonoBehaviour, IFixArea
{

    public ParticleSystem fixingEffect;
    public GameObject fixableReplacementObject;
    bool isFixing;

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

        yield return new WaitForSeconds(7);


        GOAD_WorldBeliefStates.instance.SetWorldState(GOAD_Condition.Condition, GOAD_Condition.State);
        if (undertakingObject.undertaking != null)
            undertakingObject.undertaking.TryCompleteTask(undertakingObject.task);
        player.playerInput.isInUI = false;
        player.animatePlayerScript.SetCraftAnimation(false);
        fixableReplacementObject.SetActive(true);
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
