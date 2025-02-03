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

    public List<ItemSprites> itemSpritesList = new List<ItemSprites>();
    public bool setOnVisible;
    bool isSet;
    public bool flippable = true;
    private void Start()
    {
        if (!setOnVisible) 
            SetRandomSprites();
        
    }

    private void SetRandomSprites()
    {
        bool r = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
        
        int i = UnityEngine.Random.Range(0, itemSpritesList.Count);
        if(mainImage != null)
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
            
    }

    private void OnBecameVisible()
    {
        if(!isSet && setOnVisible)
        {
            isSet = true;
            SetRandomSprites();
        }
            
    }
}
