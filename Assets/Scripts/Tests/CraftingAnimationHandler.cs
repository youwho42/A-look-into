using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingAnimationHandler : MonoBehaviour
{
    

    public Animator animator;
    public QI_Inventory inventory;
    public SpriteRenderer itemSprite;
    public AudioSource source;
    float mainVolume;
    private void Start()
    {
        GameEventManager.onInventoryUpdateEvent.AddListener(SetInventoryItemImage);
        GameEventManager.onVolumeChangedEvent.AddListener(ChangeVolume);
        mainVolume = source.volume;
    }
    private void OnDisable()
    {
        GameEventManager.onInventoryUpdateEvent.RemoveListener(SetInventoryItemImage);
        GameEventManager.onVolumeChangedEvent.RemoveListener(ChangeVolume);

    }
    void ChangeVolume()
    {
        source.volume = mainVolume * PlayerPreferencesManager.instance.GetTrackVolume(AudioTrack.Effects);
    }
    public void SetAnimation(bool active)
    {
        
        animator.SetBool("IsCrafting", active);
        if(active)
        {
            ChangeVolume();
            if(!source.isPlaying)
                source.Play();
        }
        else
            source.Stop();
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
