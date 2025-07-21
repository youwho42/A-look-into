using System;
using System.Collections.Generic;
using UnityEngine;


public class RandomSpriteForItems : MonoBehaviour
{
    [Serializable]
    public struct ItemSprites
    {
        public Sprite item;
        //public Sprite shadow;
    }
 
    public SpriteRenderer mainImage;
    public SpriteRenderer mainShadow;
    public SpriteRenderer nightShadow;

    public List<ItemSprites> itemSpritesList = new List<ItemSprites>();
    
    public bool flippable = true;

    private void Start()
    {
        SetRandomSprites();
    }
    //private void OnBecameVisible()
    //{
    //    SetRandomSprites();
    //}

    private void SetRandomSprites()
    {
        bool r = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
        
        int i = UnityEngine.Random.Range(0, itemSpritesList.Count);
        
        if (mainImage != null)
        {
            mainImage.sprite = itemSpritesList[i].item;
            if(flippable)
                mainImage.flipX = r;
            
            
        }
            
        if (mainShadow != null)
        {
            mainShadow.sprite = itemSpritesList[i].item;
            if (flippable)
                mainShadow.flipX = r;
            
        }
        if (nightShadow != null)
        {
            nightShadow.sprite = itemSpritesList[i].item;
            if (flippable)
                nightShadow.flipX = r;
            
        }

    }

    
}
