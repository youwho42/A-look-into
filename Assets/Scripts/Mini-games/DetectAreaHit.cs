using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DetectAreaHit : MonoBehaviour
{
    
    public bool isInArea;
    SpriteRenderer animalSprite;

    private void Start()
    {
        animalSprite = GetComponent<SpriteRenderer>();
        if (animalSprite != null)
            animalSprite.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("MiniGame"))
        {
            isInArea = true;
            if(animalSprite != null)
                animalSprite.enabled = true;
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("MiniGame"))
        {
            isInArea = false;
            if (animalSprite != null)
                animalSprite.enabled = false;
        }
    }

}
