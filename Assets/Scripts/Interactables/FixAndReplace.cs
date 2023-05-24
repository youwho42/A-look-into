using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAndReplace : MonoBehaviour, IFixArea
{
    public ParticleSystem fixingEffect;
    public GameObject fixableReplacementObject;
    public SpriteRenderer fixableSprite;
    bool isFixing;

    public CompleteTaskObject undertakingObject;
    
    public bool Fix(List<FixableAreaIngredient> ingredients)
    {
        if(undertakingObject.undertaking != null)
        {
            if (undertakingObject.undertaking.CurrentState != UndertakingState.Active)
            {
                NotificationManager.instance.SetNewNotification($"This doesn't work", NotificationManager.NotificationType.Warning);
                return false;
            }
        }
        

        if (!isFixing)
            StartCoroutine(FixCo(ingredients));
        return true;
    }

    IEnumerator FixCo(List<FixableAreaIngredient> ingredients)
    {
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
        {
            item.GenerateId();
        }
        
        fixableSprite.color = new Color(fixableSprite.color.r, fixableSprite.color.r, fixableSprite.color.r, 0);
        yield return new WaitForSeconds(3);
        if(fixableReplacementObject.TryGetComponent(out QI_Item data))
        {
            if (data.Data.compendiumGuide != null)
            {
                if (!PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Contains(data.Data.compendiumGuide))
                {
                    PlayerInformation.instance.playerGuidesCompendiumDatabase.Items.Add(data.Data.compendiumGuide);
                    NotificationManager.instance.SetNewNotification($"{data.Data.compendiumGuide.Name} added to guides", NotificationManager.NotificationType.Compendium);
                    GameEventManager.onGuideCompediumUpdateEvent.Invoke();
                }
            }
        }
        if (undertakingObject.undertaking != null)
            undertakingObject.undertaking.TryCompleteTask(undertakingObject.task);
        
        Destroy(this.gameObject);
    }
    void RemoveItemsFromInventory(List<FixableAreaIngredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            PlayerInformation.instance.playerInventory.RemoveItem(ingredient.item.Name, ingredient.amount);
            NotificationManager.instance.SetNewNotification($"{ingredient.amount} {ingredient.item.Name} removed", NotificationManager.NotificationType.Inventory);

        }
    }
}
