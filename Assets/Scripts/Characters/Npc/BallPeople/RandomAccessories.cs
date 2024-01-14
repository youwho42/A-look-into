using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (BallPeopleManager.instance.accessoryIndexQueue.Count <= 0)
            BallPeopleManager.instance.GenerateRandomList(accessoryList.Count);

        accessoryIndex = -1;
        System.Random random = new System.Random();
        float r = random.Next(0, 100);
        float max = 100 / accessoryList.Count;
        
        if (r > max)
            accessoryIndex = BallPeopleManager.instance.accessoryIndexQueue.Dequeue();
        
        SetAccessories(accessoryIndex);
    }

    public void SetAccessories(int index)
    {
        foreach (var item in accessoryList)
        {
            item.gameObject.SetActive(false);
        }
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
