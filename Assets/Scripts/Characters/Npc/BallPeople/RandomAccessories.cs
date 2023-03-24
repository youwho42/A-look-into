using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using UnityEngine;

public class RandomAccessories : MonoBehaviour
{
    public GameObject accessoriesHolder;
    public List<SpriteRenderer> accessoryList  = new List<SpriteRenderer>();


    [HideInInspector]
    public int accessoryIndex;

    public void PopulateList()
    {
        
        accessoryList = accessoriesHolder.GetComponentsInChildren<SpriteRenderer>().ToList();
        accessoryIndex = -1;

        foreach (var item in accessoryList)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void ChooseAccessories()
    {
        
        float r = UnityEngine.Random.Range(0.0f, 1.0f);
        if (r > 0.5f)
        {
            int rand = UnityEngine.Random.Range(0, accessoryList.Count);
            accessoryIndex = rand;
        }
        
        SetAccessories(accessoryIndex);
    }

    public void SetAccessories(int index)
    {
        accessoryIndex = index;
        if(accessoryIndex != -1)
            accessoryList[index].gameObject.SetActive(true);
    }
}
