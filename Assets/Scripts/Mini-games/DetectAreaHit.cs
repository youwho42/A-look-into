using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DetectAreaHit : MonoBehaviour
{
    
    //public bool isInArea;
    SpriteRenderer animalSprite;

    Collider2D coll;

    private void Start()
    {
        animalSprite = GetComponent<SpriteRenderer>();
        if (animalSprite != null)
            animalSprite.enabled = false;
        coll = GetComponent<Collider2D>();
    }
    public bool IsInArea()
    {
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        
        filter.useLayerMask = true;
        filter.layerMask = LayerMask.GetMask("MiniGame");
        List<Collider2D> results = new List<Collider2D>();
        coll.Overlap(filter, results);

        if (results.Count > 0)
            return true;
        return false;
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.gameObject.layer == LayerMask.NameToLayer("MiniGame"))
    //    {
    //        isInArea = true;
    //        if(animalSprite != null)
    //            animalSprite.enabled = true;
    //    }
            
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("MiniGame"))
    //    {
    //        isInArea = false;
    //        if (animalSprite != null)
    //            animalSprite.enabled = false;
    //    }
    //}

}
