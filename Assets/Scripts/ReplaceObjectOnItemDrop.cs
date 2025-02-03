using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceObjectOnItemDrop : MonoBehaviour
{
   
    public List<GameObject> grassObjects = new List<GameObject>();
    private void OnEnable()
    {
        CheckForObjects();
    }
    private void OnDisable()
    {
        ShowObjects(true);
    }
    public void CheckForObjects()
    {
        Collider2D coll = GetComponent<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.useDepth = true;
        filter.minDepth = coll.transform.position.z;
        filter.maxDepth = coll.transform.position.z;
        List<Collider2D> results = new List<Collider2D>();
        coll.Overlap(filter, results);

        grassObjects.Clear();
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

    
    public void ShowObjects(bool showState)
    {
        
        if (grassObjects.Count > 0)
        {
            foreach (var item in grassObjects)
            {
                if (item != null)
                    item.SetActive(showState);
            }
        }
        if(showState)
            grassObjects.Clear();
    }

    

}
