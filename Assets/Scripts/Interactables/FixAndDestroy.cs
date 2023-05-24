using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAndDestroy : MonoBehaviour, IFixArea
{

    public ParticleSystem fixingEffect;
    public SpriteRenderer fixableSprite;
    bool isFixing;
    public bool Fix(List<FixableAreaIngredient> ingredients)
    {
        if(!isFixing)
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
        while(timer < timeToFade)
        {
            float a = Mathf.Lerp(1, 0, timer / timeToFade);
            fixableSprite.color = new Color(fixableSprite.color.r, fixableSprite.color.r, fixableSprite.color.r, a);
            timer += Time.deltaTime;
            yield return null;
        }
        fixableSprite.color = new Color(fixableSprite.color.r, fixableSprite.color.r, fixableSprite.color.r, 0);
        yield return new WaitForSeconds(3);
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
