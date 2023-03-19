using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using UnityEngine;

public class RandomAccessories : MonoBehaviour
{
    [Serializable]
    public class Accessory
    {
        public GameObject accessoryHolder;
        public List<SpriteRenderer> accessoryList  = new List<SpriteRenderer>();
    }
    
    public List<Accessory> accessories = new List<Accessory>();

    [HideInInspector]
    public List<bool> accessoryActive = new List<bool>();
    [HideInInspector]
    public List<int> accessoryIndex = new List<int>();

    private void Start()
    {
        //foreach (var acc in accessories)
        //{
        //    PopulateList(acc);
        //}
        
        //ChooseAccessories();
        //SetAccessories(accessoryActive, accessoryIndex);
    }

    public void PopulateList()
    {
        foreach (var acc in accessories)
        {
            acc.accessoryList = acc.accessoryHolder.GetComponentsInChildren<SpriteRenderer>().ToList();
            accessoryActive.Add(false);
            accessoryIndex.Add(0);
            foreach (var item in acc.accessoryList)
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    public void ChooseAccessories()
    {
        
        for (int i = 0; i < accessories.Count; i++)
        {
            
            float r = UnityEngine.Random.Range(0.0f, 1.0f);
            if (r > 0.75f)
            {
                
                int rand = UnityEngine.Random.Range(0, accessories[i].accessoryList.Count);
                accessoryActive[i] = true;
                accessoryIndex[i] = rand;
            }
        }
        SetAccessories(accessoryActive, accessoryIndex);
    }

    public void SetAccessories(List<bool> active, List<int> index)
    {
        
        accessoryActive = new List<bool>(active);
        accessoryIndex = new List<int>(index);
        for (int i = 0; i < accessories.Count; i++)
        {
            if (active[i] == true)
                accessories[i].accessoryList[index[i]].gameObject.SetActive(true);
        }
    }
}
