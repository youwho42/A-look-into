using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAndReplace : MonoBehaviour, IFixArea
{
    public ParticleSystem fixingEffect;
    public SaveableEntity fixableReplacementObject;
    public SpriteRenderer fixableSprite;

    
    public void Fix(List<FixableAreaIngredient> ingredients)
    {
        StartCoroutine(FixCo(ingredients));
    }

    IEnumerator FixCo(List<FixableAreaIngredient> ingredients)
    {
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
        SaveableEntity s = Instantiate(fixableReplacementObject, transform.position, Quaternion.identity);
        s.GenerateId();
        fixableSprite.color = new Color(fixableSprite.color.r, fixableSprite.color.r, fixableSprite.color.r, 0);
        yield return new WaitForSeconds(3);
        
        Destroy(this.gameObject);
    }
    void RemoveItemsFromInventory(List<FixableAreaIngredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            PlayerInformation.instance.playerInventory.RemoveItem(ingredient.item.Name, ingredient.amount);

        }
        NotificationManager.instance.SetNewNotification("Items removed from inventory");
    }
}
