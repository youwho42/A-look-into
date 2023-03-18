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

    private void Start()
    {
        foreach (var acc in accessories)
        {
            PopulateList(acc);
        }
        
        ChooseAccessories();
    }

    void PopulateList(Accessory acc)
    {
        acc.accessoryList = acc.accessoryHolder.GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    void ChooseAccessories()
    {
        foreach (var acc in accessories)
        {
            foreach (var item in acc.accessoryList)
            {
                item.gameObject.SetActive(false);
            }
            float r = UnityEngine.Random.Range(0.0f, 1.0f);
            if (r > 0.85f)
            {
                int rand = UnityEngine.Random.Range(0, acc.accessoryList.Count);
                acc.accessoryList[rand].gameObject.SetActive(true);
            }
        }
    }
}
