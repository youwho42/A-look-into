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
        System.Random random = new System.Random();
        float r = random.Next(0, 100);
        //r = r / 100;
        float max = 100 / accessoryList.Count;
        //float r = UnityEngine.Random.Range(0.0f, 1.0f);
        if (r > max)
        {
            System.Random rand = new System.Random();
            int ra = rand.Next(0, accessoryList.Count);
            
            if(ra == BallPeopleManager.instance.lastAccessoryIndex)
            {
                ChooseAccessories();
                return;
            }
                
            //int rand = UnityEngine.Random.Range(0, accessoryList.Count);
            accessoryIndex = ra;
            BallPeopleManager.instance.lastAccessoryIndex = ra;
        }
        
        SetAccessories(accessoryIndex);
    }

    public void SetAccessories(int index)
    {
        accessoryIndex = index;
        if(accessoryIndex != -1)
            accessoryList[index].gameObject.SetActive(true);
        
    }

    public int GetAccessory(SpriteRenderer sprite)
    {
        for (int i = 0; i < accessoryList.Count; i++)
        {
            if (accessoryList[i] == sprite)
                return i;
        }
        return -1;
    }
}
