using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingAnimationHandler : MonoBehaviour
{
    

    public Animator animator;
    public QI_Inventory inventory;
    public SpriteRenderer itemSprite;
    public SpriteRenderer craftingItemSprite;
    public SpriteRenderer ColoredSprite;
    public AudioSource source;
    float mainVolume;
    private void OnEnable()
    {
        GameEventManager.onInventoryUpdateEvent.AddListener(SetInventoryItemImage);
        mainVolume = source.volume;
        SetInventoryItemImage();
    }
    private void OnDisable()
    {
        GameEventManager.onInventoryUpdateEvent.RemoveListener(SetInventoryItemImage);

    }
    
    public void SetAnimation(bool active, Color spriteColor, Sprite itemIcon = null)
    {
        
        animator.SetBool("IsCrafting", active);
        if(active)
        {
            if(!source.isPlaying)
                source.Play();
            if (itemIcon != null && craftingItemSprite != null)
                craftingItemSprite.sprite = itemIcon;
            if(ColoredSprite != null)
                ColoredSprite.color = spriteColor;
        }
        else
        {
            source.Stop();
            if (craftingItemSprite != null)
                craftingItemSprite.sprite = null;
        }
            
    }

    public void SetInventoryItemImage()
    {
        if (inventory.Stacks.Count > 0)
            itemSprite.sprite = inventory.Stacks[0].Item.Icon;
        else
            itemSprite.sprite = null;
        
    }

}
