using UnityEngine;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using Klaxon.SaveSystem;
using System.Collections;
using Klaxon.Interactable;


public class PokableItemReplace : PokableItem
{
    public QI_ItemData replacedItem;
    public ParticleSystem particles;

    

    public override void PokeItemSuccess()
    {
        base.PokeItemSuccess();
        StartCoroutine(ReplaceItemCo());

    }

    IEnumerator ReplaceItemCo()
    {
        if (replacedItem != null)
        {
            int r = Random.Range(0, replacedItem.ItemPrefabVariants.Count);
            var go = Instantiate(replacedItem.ItemPrefabVariants[r], transform.position, Quaternion.identity, transform.parent);
            if (go.TryGetComponent(out SaveableItemEntity item))
                item.GenerateId();
        }
        if(TryGetComponent(out Interactable interactable))
        {
            interactable.canInteract = false;
            interactable.visualItem.gameObject.SetActive(false);
        }
            

        yield return new WaitForSeconds(0.8f);

        AudioManager.instance.PlaySound("GlassCrack");
        
        if (particles != null)
        {
            particles.gameObject.SetActive(true);
            particles.Play();
        }
        
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);


        yield return null;
    }

    public override void PokeItemFail()
    {
        base.PokeItemFail();

    }


}
