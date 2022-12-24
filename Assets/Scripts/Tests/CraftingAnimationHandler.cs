using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingAnimationHandler : MonoBehaviour
{
    

    public Animator animator;
    public QI_Inventory inventory;
    public SpriteRenderer itemSprite;
    public AudioSource audio;
    private void Start()
    {
        GameEventManager.onInventoryUpdateEvent.AddListener(SetInventoryItemImage);
    }
    private void OnDisable()
    {
        GameEventManager.onInventoryUpdateEvent.RemoveListener(SetInventoryItemImage);
    }
    public void SetAnimation(bool active)
    {
        animator.SetBool("IsCrafting", active);
        if(active)
        {
            if(!audio.isPlaying)
                audio.Play();
        }
        else
            audio.Stop();
    }

    public void SetInventoryItemImage()
    {
        if (inventory.Stacks.Count > 0)
        {
            itemSprite.sprite = inventory.Stacks[0].Item.Icon;
        }
        else
        {
            itemSprite.sprite = null;
        }
    }

}
