using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceObjectOnItemDrop : MonoBehaviour
{
   
    public List<GameObject> grassObjects = new List<GameObject>();
    private void Start()
    {
        CheckForObjects();
    }
    public void CheckForObjects()
    {
        Collider2D coll = GetComponentInChildren<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        
        List<Collider2D> results = new List<Collider2D>();
        coll.OverlapCollider(filter, results);

        
        if (results.Count > 0)
        {
            foreach (var item in results)
            {
                if (!item.CompareTag("Grass"))
                    continue;

                grassObjects.Add(item.gameObject);
            }
            ShowObjects(false);
        }

    }

    //void HideObjects()
    //{
    //    if(grassObjects.Count > 0)
    //    {
    //        foreach (var item in grassObjects)
    //        {
    //            item.SetActive(false);
    //        }
    //    }
    //}
    public void ShowObjects(bool showState)
    {
        
        if (grassObjects.Count > 0)
        {
            foreach (var item in grassObjects)
            {
                item.SetActive(showState);
            }
        }
        if(showState)
            grassObjects.Clear();
    }

    

}
