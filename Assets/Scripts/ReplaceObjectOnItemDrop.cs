using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceObjectOnItemDrop : MonoBehaviour
{
   
    public List<GameObject> grassObjects = new List<GameObject>();
    Collider2D coll;
    private void OnEnable()
    {
        coll = GetComponent<Collider2D>();
        CheckForObjects();
    }
    private void OnDisable()
    {
        ShowObjects(true);
    }
    public void CheckForObjects()
    {

        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.useDepth = true;
        filter.minDepth = coll.gameObject.transform.position.z;
        filter.maxDepth = coll.gameObject.transform.position.z;
        List<Collider2D> results = new List<Collider2D>();
        coll.Overlap(filter, results);

        
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

    //public void CheckObjectsDelay()
    //{
    //    Invoke("CheckForObjects", 0.1f);
    //}
    public void ShowObjects(bool showState)
    {
        
        foreach (var item in grassObjects)
        {
            if (item != null)
                item.SetActive(showState);
        }
        
        if(showState)
            grassObjects.Clear();
        //if(showState)
        //    grassObjects.Clear();
    }

    

}
